using Microsoft.EntityFrameworkCore;
using WebhookEngine.Domain;

namespace WebhookEngine.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Endpoint> Endpoints => Set<Endpoint>();
    public DbSet<WebhookEventRecord> WebhookEvents => Set<WebhookEventRecord>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<DeliveryAttempt> DeliveryAttempts => Set<DeliveryAttempt>();
    public DbSet<DeadLetterEntry> DeadLetterEntries => Set<DeadLetterEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
