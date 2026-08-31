using Capri.Sgr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capri.Sgr.Infrastructure.Data.Configurations;

public sealed class AssociateMembershipConfiguration : IEntityTypeConfiguration<AssociateMembership>
{
    public void Configure(EntityTypeBuilder<AssociateMembership> builder)
    {
        builder.ToTable("AssociateMemberships"); builder.HasKey(x => x.Id);
        builder.Property(x => x.AssociateId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.PersonKind).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.PrefixKind).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.PrefixDisplayValue).HasMaxLength(200);
        builder.Property(x => x.PrefixNormalizedValue).HasMaxLength(200);
        builder.HasIndex(x => x.AssociateId).IsUnique();
        builder.HasIndex(x => x.PrefixNormalizedValue).IsUnique().HasFilter("\"PrefixNormalizedValue\" IS NOT NULL");
    }
}

public sealed class PrefixProposalConfiguration : IEntityTypeConfiguration<PrefixProposal>
{
    public void Configure(EntityTypeBuilder<PrefixProposal> builder)
    {
        builder.ToTable("PrefixProposals"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Kind).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.DisplayValue).HasMaxLength(200);
        builder.Property(x => x.NormalizedValue).HasMaxLength(200);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.HasIndex(x => x.MembershipId);
    }
}

public sealed class ReclassificationProposalConfiguration : IEntityTypeConfiguration<ReclassificationProposal>
{
    public void Configure(EntityTypeBuilder<ReclassificationProposal> builder)
    {
        builder.ToTable("ReclassificationProposals"); builder.HasKey(x => x.Id);
        builder.Property(x => x.ProposedType).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.HasIndex(x => new { x.MembershipId, x.Status }).IsUnique().HasFilter("\"Status\" = 'Open'");
    }
}