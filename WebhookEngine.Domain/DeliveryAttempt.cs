namespace WebhookEngine.Domain;

public sealed class DeliveryAttempt
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid DeliveryId { get; init; }
    public int AttemptNumber { get; init; }
    public int? HttpStatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTimeOffset AttemptedAt { get; init; } = DateTimeOffset.UtcNow;
}
