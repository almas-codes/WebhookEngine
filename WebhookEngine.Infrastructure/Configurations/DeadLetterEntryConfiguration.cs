using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebhookEngine.Domain;

namespace WebhookEngine.Infrastructure.Configurations;

public sealed class DeadLetterEntryConfiguration : IEntityTypeConfiguration<DeadLetterEntry>
{
    public void Configure(EntityTypeBuilder<DeadLetterEntry> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne<Delivery>()
               .WithMany()
               .HasForeignKey(e => e.DeliveryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
