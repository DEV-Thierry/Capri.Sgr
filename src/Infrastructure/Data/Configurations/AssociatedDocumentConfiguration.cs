using Capri.Sgr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capri.Sgr.Infrastructure.Data.Configurations;

public sealed class AssociatedDocumentConfiguration : IEntityTypeConfiguration<AssociatedDocument>
{
    public void Configure(EntityTypeBuilder<AssociatedDocument> builder)
    {
        builder.ToTable("AssociatedDocuments");
        builder.HasKey(document => document.Id);
        builder.Property(document => document.DossierId).HasMaxLength(200).IsRequired();
        builder.Property(document => document.DocumentType).HasMaxLength(200).IsRequired();
        builder.HasIndex(document => new { document.DossierId, document.DocumentType });

        builder.HasMany(document => document.Versions)
            .WithOne()
            .HasForeignKey("AssociatedDocumentId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AssociatedDocumentVersionConfiguration : IEntityTypeConfiguration<AssociatedDocumentVersion>
{
    public void Configure(EntityTypeBuilder<AssociatedDocumentVersion> builder)
    {
        builder.ToTable("AssociatedDocumentVersions");
        builder.HasKey(version => version.Id);
        builder.Property<Guid>("AssociatedDocumentId").IsRequired();
        builder.Property(version => version.Number).IsRequired();
        builder.HasIndex("AssociatedDocumentId", nameof(AssociatedDocumentVersion.Number)).IsUnique();
        builder.Property(version => version.AuthorId).HasMaxLength(450).IsRequired();
        builder.Property(version => version.OriginalFileName).HasMaxLength(500).IsRequired();
        builder.Property(version => version.ContentType).HasMaxLength(200).IsRequired();
        builder.Property(version => version.ContentHash).HasMaxLength(200).IsRequired();
        builder.Property(version => version.StorageReference).HasMaxLength(1000);
        builder.Property(version => version.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(version => version.Reason).HasMaxLength(4000);
        builder.Property(version => version.DecisionAuthorId).HasMaxLength(450);
    }
}
