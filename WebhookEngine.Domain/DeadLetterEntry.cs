namespace WebhookEngine.Domain;

public sealed class DeadLetterEntry
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid DeliveryId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTimeOffset DeadLetteredAt { get; init; } = DateTimeOffset.UtcNow;
}
