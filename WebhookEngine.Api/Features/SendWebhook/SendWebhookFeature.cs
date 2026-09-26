using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebhookEngine.Core.Abstractions;
using WebhookEngine.Domain;

namespace WebhookEngine.Api.Features.SendWebhook;

public record SendWebhookRequest(Guid TenantId, string EventType, string PayloadJson, string IdempotencyKey);

public static class SendWebhookFeature
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/webhooks/send", async (SendWebhookRequest request, IWebhookClient client, CancellationToken ct) =>
        {
            var webhookEvent = new WebhookEventRecord
            {
                TenantId = request.TenantId,
                Type = request.EventType,
                PayloadJson = request.PayloadJson,
                IdempotencyKey = request.IdempotencyKey
            };

            await client.SendAsync(webhookEvent, ct);

            return Results.Accepted();
        })
        .WithTags("Webhooks");
    }
}
