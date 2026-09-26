using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebhookEngine.Domain;

namespace WebhookEngine.Infrastructure.Configurations;

public sealed class EndpointConfiguration : IEntityTypeConfiguration<Endpoint>
{
    public void Configure(EntityTypeBuilder<Endpoint> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Url).IsRequired().HasMaxLength(2000);
        builder.Property(e => e.SigningSecretHash).IsRequired().HasMaxLength(256);
        builder.Property(e => e.RotatingSecretHash).HasMaxLength(256);
        builder.Property(e => e.Status).HasConversion<string>();
        // Postgres array mapping for subscribed events
    }
}
