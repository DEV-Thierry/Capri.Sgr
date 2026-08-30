using Capri.Sgr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capri.Sgr.Infrastructure.Data.Configurations;

public sealed class DerivedEffectExecutionConfiguration : IEntityTypeConfiguration<DerivedEffectExecution>
{
    public void Configure(EntityTypeBuilder<DerivedEffectExecution> builder)
    {
        builder.ToTable("DerivedEffectExecutions");
        builder.HasKey(execution => execution.Id);
        builder.Property(execution => execution.ExecutionKey).HasMaxLength(500).IsRequired();
        builder.HasIndex(execution => execution.ExecutionKey).IsUnique();
        builder.Property(execution => execution.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(execution => execution.Failure).HasMaxLength(4000);
    }
}
