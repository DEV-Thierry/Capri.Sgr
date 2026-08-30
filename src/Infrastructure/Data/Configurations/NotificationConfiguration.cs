using Capri.Sgr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capri.Sgr.Infrastructure.Data.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(notification => notification.Id);
        builder.Property(notification => notification.EventType).HasMaxLength(200).IsRequired();
        builder.Property(notification => notification.Recipient).HasMaxLength(500).IsRequired();
        builder.Property(notification => notification.Channel).HasMaxLength(100).IsRequired();
        builder.Property(notification => notification.Body).HasMaxLength(8000).IsRequired();
        builder.Property(notification => notification.Failure).HasMaxLength(4000);
        builder.Property(notification => notification.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
    }
}
