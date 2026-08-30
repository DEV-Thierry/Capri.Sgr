using Capri.Sgr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capri.Sgr.Infrastructure.Data.Configurations;

public sealed class PendingItemConfiguration : IEntityTypeConfiguration<PendingItem>
{
    public void Configure(EntityTypeBuilder<PendingItem> builder)
    {
        builder.ToTable("PendingItems");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Cause).HasMaxLength(1000).IsRequired();
        builder.Property(item => item.Impact).HasMaxLength(2000).IsRequired();
        builder.Property(item => item.Regularization).HasMaxLength(2000).IsRequired();
        builder.Property(item => item.ClosureReason).HasMaxLength(2000);
        builder.Property(item => item.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
    }
}
