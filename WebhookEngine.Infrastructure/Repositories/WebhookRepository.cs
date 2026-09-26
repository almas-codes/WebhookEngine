using Microsoft.EntityFrameworkCore;
using WebhookEngine.Core.Abstractions;
using WebhookEngine.Domain;

namespace WebhookEngine.Infrastructure.Repositories;

public sealed class WebhookRepository : IWebhookRepository
{
    private readonly AppDbContext _dbContext;

    public WebhookRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddDeliveryAsync(WebhookEngine.Domain.Delivery delivery, CancellationToken cancellationToken = default)
    {
        _dbContext.Deliveries.Add(delivery);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddEndpointAsync(Endpoint endpoint, CancellationToken cancellationToken = default)
    {
        _dbContext.Endpoints.Add(endpoint);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddEventAsync(WebhookEventRecord webhookEvent, CancellationToken cancellationToken = default)
    {
        _dbContext.WebhookEvents.Add(webhookEvent);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Endpoint>> GetEndpointsForEventAsync(Guid tenantId, string eventType, CancellationToken cancellationToken = default)
    {
        // Simple matching logic. In real-world, we check SubscribedEventPatterns properly.
        var endpoints = await _dbContext.Endpoints
            .Where(e => e.TenantId == tenantId && e.Status == EndpointStatus.Active)
            .ToListAsync(cancellationToken);

        // Filter in memory for patterns like "invoice.*"
        return endpoints.Where(e => e.SubscribedEventPatterns.Any(p => MatchPattern(p, eventType))).ToList();
    }

    private static bool MatchPattern(string pattern, string eventType)
    {
        if (pattern == "*") return true;
        if (pattern.EndsWith(".*"))
        {
            var prefix = pattern.Substring(0, pattern.Length - 2);
            return eventType.StartsWith(prefix);
        }
        return pattern == eventType;
    }
}
