using WebhookEngine.Domain;

namespace WebhookEngine.Core.Abstractions;

public interface IWebhookClient
{
    Task SendAsync(WebhookEventRecord webhookEvent, CancellationToken cancellationToken = default);
    Task RegisterEndpointAsync(Endpoint endpoint, CancellationToken cancellationToken = default);
}
