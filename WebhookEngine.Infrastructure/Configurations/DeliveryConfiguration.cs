using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebhookEngine.Domain;

namespace WebhookEngine.Infrastructure.Configurations;

public sealed class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Status).HasConversion<string>();
        
        builder.HasIndex(e => e.NextAttemptAt)
               .HasFilter("\"Status\" = 'Pending'");
    }
}
