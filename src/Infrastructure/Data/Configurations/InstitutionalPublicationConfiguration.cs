using Capri.Sgr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capri.Sgr.Infrastructure.Data.Configurations;

public sealed class InstitutionalPublicationConfiguration : IEntityTypeConfiguration<InstitutionalPublication>
{
    public void Configure(EntityTypeBuilder<InstitutionalPublication> builder)
    {
        builder.ToTable("InstitutionalPublications");
        builder.HasKey(publication => publication.Id);
        builder.Property(publication => publication.Category).HasMaxLength(100).IsRequired();
        builder.Property(publication => publication.AuthorId).HasMaxLength(450).IsRequired();
        builder.Property(publication => publication.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.HasIndex(publication => new { publication.Status, publication.Category, publication.PublishedAt });
        builder.OwnsMany(publication => publication.Versions, version =>
        {
            version.ToTable("InstitutionalPublicationVersions");
            version.WithOwner().HasForeignKey("InstitutionalPublicationId");
            version.HasKey(item => item.Id);
            version.Property(item => item.Title).HasMaxLength(300).IsRequired();
            version.Property(item => item.Summary).HasMaxLength(1000).IsRequired();
            version.Property(item => item.Body).IsRequired();
            version.Property(item => item.AuthorId).HasMaxLength(450).IsRequired();
            version.HasIndex("InstitutionalPublicationId", nameof(InstitutionalPublicationVersion.Number)).IsUnique();
        });
    }
}
