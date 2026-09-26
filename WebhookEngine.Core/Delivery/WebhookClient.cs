using WebhookEngine.Core.Abstractions;
using WebhookEngine.Domain;

namespace WebhookEngine.Core.Delivery;

public class WebhookClient : IWebhookClient
{
    private readonly IWebhookRepository _repository;

    public WebhookClient(IWebhookRepository repository)
    {
        _repository = repository;
    }

    public async Task RegisterEndpointAsync(Endpoint endpoint, CancellationToken cancellationToken = default)
    {
        await _repository.AddEndpointAsync(endpoint, cancellationToken);
    }

    public async Task SendAsync(WebhookEventRecord webhookEvent, CancellationToken cancellationToken = default)
    {
        // Save the event
        await _repository.AddEventAsync(webhookEvent, cancellationToken);

        // Find matching endpoints for this tenant and event type
        var endpoints = await _repository.GetEndpointsForEventAsync(webhookEvent.TenantId, webhookEvent.Type, cancellationToken);

        // Create deliveries for each endpoint
        foreach (var endpoint in endpoints)
        {
            var delivery = new WebhookEngine.Domain.Delivery
            {
                EventId = webhookEvent.Id,
                EndpointId = endpoint.Id,
                Status = DeliveryStatus.Pending,
                NextAttemptAt = DateTimeOffset.UtcNow
            };
            await _repository.AddDeliveryAsync(delivery, cancellationToken);
        }
    }
}
