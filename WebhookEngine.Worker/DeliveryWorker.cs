using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using WebhookEngine.Domain;
using WebhookEngine.Infrastructure;
using System.Net.Http;
using System.Text;
using WebhookEngine.Core.Signing;

namespace WebhookEngine.Worker;

public class DeliveryWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DeliveryWorker> _logger;

    public DeliveryWorker(IServiceProvider serviceProvider, ILogger<DeliveryWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DeliveryWorker started.");

        // keep polling until the app shuts down
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingDeliveriesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                // don't crash the worker if one cycle fails
                _logger.LogError(ex, "Error processing deliveries.");
            }

            // aggressive polling for the demo, would be higher in prod
            await Task.Delay(2000, stoppingToken);
        }
    }

    private async Task ProcessPendingDeliveriesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

        // grab a batch of pending webhooks that are ready to go
        var pendingDeliveries = await dbContext.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending && d.NextAttemptAt <= DateTimeOffset.UtcNow)
            .OrderBy(d => d.NextAttemptAt)
            .Take(50)
            .ToListAsync(cancellationToken);

        if (!pendingDeliveries.Any()) return;

        foreach (var delivery in pendingDeliveries)
        {
            await ProcessDeliveryAsync(delivery, dbContext, httpClientFactory, cancellationToken);
        }
    }

    private async Task ProcessDeliveryAsync(Delivery delivery, AppDbContext dbContext, IHttpClientFactory httpClientFactory, CancellationToken cancellationToken)
    {
        var endpoint = await dbContext.Endpoints.FindAsync(new object[] { delivery.EndpointId }, cancellationToken);
        var webhookEvent = await dbContext.WebhookEvents.FindAsync(new object[] { delivery.EventId }, cancellationToken);

        if (endpoint == null || webhookEvent == null) return;

        delivery.AttemptCount++;
        var attempt = new DeliveryAttempt { DeliveryId = delivery.Id, AttemptNumber = delivery.AttemptCount };

        try
        {
            var client = httpClientFactory.CreateClient("WebhookClient");
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint.Url);
            
            var payload = webhookEvent.PayloadJson;
            // compute signature so the receiver knows it actually came from us
            var signature = HmacSigner.GenerateSignature(payload, "MasterSecretKey123!"); 
            
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            request.Headers.Add("X-Webhook-Signature", signature);
            request.Headers.Add("X-Webhook-Timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            request.Headers.Add("Event-Id", webhookEvent.IdempotencyKey);

            var startTime = DateTimeOffset.UtcNow;
            var response = await client.SendAsync(request, cancellationToken);
            attempt.Duration = DateTimeOffset.UtcNow - startTime;

            attempt.HttpStatusCode = (int)response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                delivery.Status = DeliveryStatus.Delivered;
                delivery.DeliveredAt = DateTimeOffset.UtcNow;
            }
            else
            {
                HandleFailure(delivery, attempt, $"HTTP {(int)response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // catch network errors or timeouts
            HandleFailure(delivery, attempt, ex.Message);
        }

        dbContext.DeliveryAttempts.Add(attempt);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private void HandleFailure(Delivery delivery, DeliveryAttempt attempt, string error)
    {
        attempt.ErrorMessage = error;

        // stop trying after 8 attempts (roughly maps to standard DLQ patterns)
        if (delivery.AttemptCount >= 8)
        {
            delivery.Status = DeliveryStatus.DeadLettered;
            delivery.NextAttemptAt = null;
        }
        else
        {
            // simple exponential backoff: 2s, 4s, 8s, etc.
            var backoffSeconds = Math.Pow(2, delivery.AttemptCount);
            delivery.NextAttemptAt = DateTimeOffset.UtcNow.AddSeconds(backoffSeconds);
        }
    }
}
