namespace WebhookEngine.Domain;

public enum EndpointStatus
{
    Active,
    Paused,
    Disabled
}

public sealed class Endpoint
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public required string Url { get; init; }
    public required string SigningSecretHash { get; init; }
    public string? RotatingSecretHash { get; init; }
    public List<string> SubscribedEventPatterns { get; init; } = new();
    public EndpointStatus Status { get; set; } = EndpointStatus.Active;
    public double HealthScore { get; set; } = 1.0;
    public int RateLimitPerMinute { get; set; } = 60;
}
