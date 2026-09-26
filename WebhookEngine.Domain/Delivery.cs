namespace WebhookEngine.Domain;

public enum DeliveryStatus
{
    Pending,
    Delivered,
    Failed,
    DeadLettered
}

public sealed class Delivery
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid EventId { get; init; }
    public Guid EndpointId { get; init; }
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    public int AttemptCount { get; set; } = 0;
    public DateTimeOffset? NextAttemptAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeliveredAt { get; set; }
}
