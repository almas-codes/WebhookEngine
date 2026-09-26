using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using WebhookEngine.Api.Features.RegisterEndpoint;
using WebhookEngine.Api.Features.SendWebhook;
using WebhookEngine.Domain;
using WebhookEngine.Infrastructure;

namespace WebhookEngine.IntegrationTests;

public class WebhookDeliveryTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public WebhookDeliveryTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteFlow_RegisterEndpoint_SendWebhook_CreatesDelivery()
    {
        // 1. Register Endpoint
        var tenantId = Guid.NewGuid();
        var registerReq = new RegisterEndpointRequest(tenantId, "https://httpbin.org/post", new List<string> { "invoice.*" });
        
        var registerRes = await _client.PostAsJsonAsync("/api/endpoints", registerReq);
        registerRes.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // 2. Send Webhook
        var sendReq = new SendWebhookRequest(tenantId, "invoice.created", "{\"amount\": 100}", Guid.NewGuid().ToString("N"));
        var sendRes = await _client.PostAsJsonAsync("/api/webhooks/send", sendReq);
        sendRes.StatusCode.Should().Be(HttpStatusCode.Accepted);

        // 3. Verify Database State
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var deliveries = db.Deliveries.ToList();
        deliveries.Should().HaveCount(1);
        deliveries[0].Status.Should().Be(DeliveryStatus.Pending);
    }
}
