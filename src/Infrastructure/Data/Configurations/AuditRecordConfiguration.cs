using Capri.Sgr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capri.Sgr.Infrastructure.Data.Configurations;

public sealed class AuditRecordConfiguration : IEntityTypeConfiguration<AuditRecord>
{
    public void Configure(EntityTypeBuilder<AuditRecord> builder)
    {
        builder.ToTable("AuditRecords");
        builder.HasKey(record => record.Id);
        builder.Property(record => record.ActorId).HasMaxLength(450);
        builder.Property(record => record.OccurredAt).IsRequired();
        builder.Property(record => record.Channel).HasMaxLength(100).IsRequired();
        builder.Property(record => record.EntityType).HasMaxLength(200).IsRequired();
        builder.Property(record => record.EntityId).HasMaxLength(200).IsRequired();
        builder.Property(record => record.Action).HasMaxLength(200).IsRequired();
        builder.Property(record => record.Reason).HasMaxLength(4000);
        builder.Property(record => record.Before).HasColumnType("jsonb");
        builder.Property(record => record.After).HasColumnType("jsonb");

        // Scheduler/outbox dispatch and granular internal permissions are deliberately deferred to later issues.
        // The application exposes Auditoria only through IAuditStore; updates/deletes are intentionally absent.
    }
}
