using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebhookEngine.Domain;

namespace WebhookEngine.Infrastructure.Configurations;

public sealed class DeliveryAttemptConfiguration : IEntityTypeConfiguration<DeliveryAttempt>
{
    public void Configure(EntityTypeBuilder<DeliveryAttempt> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne<Delivery>()
               .WithMany()
               .HasForeignKey(e => e.DeliveryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
