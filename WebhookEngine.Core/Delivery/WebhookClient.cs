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
        // save the raw event payload first so we have a durable record
        await _repository.AddEventAsync(webhookEvent, cancellationToken);

        // find all active endpoints that actually want to hear about this event
        var endpoints = await _repository.GetEndpointsForEventAsync(webhookEvent.TenantId, webhookEvent.Type, cancellationToken);

        // queue up a delivery attempt for each matched endpoint
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
