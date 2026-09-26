using WebhookEngine.Domain;

namespace WebhookEngine.Core.Abstractions;

public interface IWebhookRepository
{
    Task AddEventAsync(WebhookEventRecord webhookEvent, CancellationToken cancellationToken = default);
    Task AddEndpointAsync(Endpoint endpoint, CancellationToken cancellationToken = default);
    Task<List<Endpoint>> GetEndpointsForEventAsync(Guid tenantId, string eventType, CancellationToken cancellationToken = default);
    Task AddDeliveryAsync(WebhookEngine.Domain.Delivery delivery, CancellationToken cancellationToken = default);
}
