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
        // grab all active endpoints for this tenant first
        var endpoints = await _dbContext.Endpoints
            .Where(e => e.TenantId == tenantId && e.Status == EndpointStatus.Active)
            .ToListAsync(cancellationToken);

        // check which ones actually subscribed to this specific event (e.g. "invoice.created")
        return endpoints.Where(e => e.SubscribedEventPatterns.Any(p => MatchPattern(p, eventType))).ToList();
    }

    private static bool MatchPattern(string pattern, string eventType)
    {
        // catch-all wildcard
        if (pattern == "*") return true;
        
        // prefix wildcard (e.g., "invoice.*")
        if (pattern.EndsWith(".*"))
        {
            var prefix = pattern.Substring(0, pattern.Length - 2);
            return eventType.StartsWith(prefix);
        }
        
        // exact match
        return pattern == eventType;
    }
}
