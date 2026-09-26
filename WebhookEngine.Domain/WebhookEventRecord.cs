namespace WebhookEngine.Domain;

public sealed class WebhookEventRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public required string Type { get; init; }
    public required string PayloadJson { get; init; }
    public int SchemaVersion { get; init; } = 1;
    public required string IdempotencyKey { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
