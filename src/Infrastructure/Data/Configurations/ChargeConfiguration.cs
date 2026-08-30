using Capri.Sgr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capri.Sgr.Infrastructure.Data.Configurations;

public sealed class ChargeConfiguration : IEntityTypeConfiguration<Charge>
{
    public void Configure(EntityTypeBuilder<Charge> builder)
    {
        builder.ToTable("Charges");
        builder.HasKey(charge => charge.Id);
        builder.Property(charge => charge.PayerId).HasMaxLength(450).IsRequired();
        builder.Property(charge => charge.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(charge => charge.Status).HasConversion<string>().HasMaxLength(32).IsRequired();

        builder.OwnsMany(charge => charge.GeneratorFacts, facts =>
        {
            facts.ToTable("ChargeGeneratorFacts");
            facts.WithOwner().HasForeignKey("ChargeId");
            facts.Property<Guid>("Id");
            facts.HasKey("Id");
            facts.Property(fact => fact.SourceType).HasMaxLength(200).IsRequired();
            facts.Property(fact => fact.SourceId).HasMaxLength(200).IsRequired();
            facts.Property(fact => fact.Kind).HasMaxLength(200).IsRequired();
            facts.HasIndex(fact => new { fact.SourceType, fact.SourceId, fact.Kind }).IsUnique();
        });

        builder.OwnsOne(charge => charge.Payment, payment =>
        {
            payment.Property(value => value.Reference).HasMaxLength(200).HasColumnName("PaymentReference");
            payment.Property(value => value.Evidence).HasMaxLength(4000).HasColumnName("PaymentEvidence");
            payment.Property(value => value.RecordedAt).HasColumnName("PaymentRecordedAt");
        });

        builder.OwnsOne(charge => charge.Cancellation, cancellation =>
        {
            cancellation.Property(value => value.Reason).HasMaxLength(4000).HasColumnName("CancellationReason");
            cancellation.Property(value => value.Evidence).HasMaxLength(4000).HasColumnName("CancellationEvidence");
            cancellation.Property(value => value.DecidedAt).HasColumnName("CancelledAt");
        });

        builder.OwnsOne(charge => charge.RefundDecision, refund =>
        {
            refund.Property(value => value.Reason).HasMaxLength(4000).HasColumnName("RefundReason");
            refund.Property(value => value.Evidence).HasMaxLength(4000).HasColumnName("RefundEvidence");
            refund.Property(value => value.DecidedAt).HasColumnName("RefundedAt");
        });
    }
}
