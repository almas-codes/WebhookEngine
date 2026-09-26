using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebhookEngine.Domain;

namespace WebhookEngine.Infrastructure.Configurations;

public sealed class WebhookEventRecordConfiguration : IEntityTypeConfiguration<WebhookEventRecord>
{
    public void Configure(EntityTypeBuilder<WebhookEventRecord> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Type).IsRequired().HasMaxLength(200);
        builder.Property(e => e.PayloadJson).IsRequired().HasColumnType("jsonb");
        builder.Property(e => e.IdempotencyKey).IsRequired().HasMaxLength(100);
        
        builder.HasIndex(e => new { e.TenantId, e.IdempotencyKey }).IsUnique();
    }
}
