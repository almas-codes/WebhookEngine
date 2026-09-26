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
            // construct the event record that will be pushed to the DB
            var webhookEvent = new WebhookEventRecord
            {
                TenantId = request.TenantId,
                Type = request.EventType,
                PayloadJson = request.PayloadJson,
                IdempotencyKey = request.IdempotencyKey
            };

            // delegates the routing and queueing to the core engine
            await client.SendAsync(webhookEvent, ct);

            // always return 202 accepted since delivery is async
            return Results.Accepted();
        })
        .WithTags("Webhooks");
    }
}
