using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebhookEngine.Core.Abstractions;
using WebhookEngine.Core.Signing;
using WebhookEngine.Domain;

namespace WebhookEngine.Api.Features.RegisterEndpoint;

public record RegisterEndpointRequest(Guid TenantId, string Url, List<string> SubscribedEvents);

public static class RegisterEndpointFeature
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/endpoints", async (RegisterEndpointRequest request, IWebhookClient client, CancellationToken ct) =>
        {
            // generate a fresh signing secret for this endpoint
            var rawSecret = Guid.NewGuid().ToString("N");
            // in real life, you'd pull this master key from a vault or config
            var hashedSecret = HmacSigner.GenerateSignature(rawSecret, "MasterSecretKey123!"); 

            var endpoint = new WebhookEngine.Domain.Endpoint
            {
                TenantId = request.TenantId,
                Url = request.Url,
                SigningSecretHash = hashedSecret,
                SubscribedEventPatterns = request.SubscribedEvents
            };

            await client.RegisterEndpointAsync(endpoint, ct);

            // only show the raw secret once, immediately upon creation
            return Results.Ok(new { endpoint.Id, Secret = rawSecret });
        })
        .WithTags("Endpoints");
    }
}
