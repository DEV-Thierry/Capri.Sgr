using Capri.Sgr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capri.Sgr.Infrastructure.Data.Configurations;

public sealed class ResponsiblePersonConfiguration : IEntityTypeConfiguration<ResponsiblePerson>
{
    public void Configure(EntityTypeBuilder<ResponsiblePerson> builder)
    {
        builder.ToTable("ResponsiblePeople");
        builder.HasKey(person => person.Id);
        builder.Property(person => person.Cpf).HasMaxLength(11).IsRequired();
        builder.HasIndex(person => person.Cpf).IsUnique();
        builder.Property(person => person.FullName).HasMaxLength(300).IsRequired();
        builder.Property(person => person.Email).HasMaxLength(320).IsRequired();
        builder.Property(person => person.Phone).HasMaxLength(50).IsRequired();
    }
}

public sealed class LegalEntityMembershipApplicationConfiguration : IEntityTypeConfiguration<LegalEntityMembershipApplication>
{
    public void Configure(EntityTypeBuilder<LegalEntityMembershipApplication> builder)
    {
        builder.ToTable("LegalEntityMembershipApplications");
        builder.HasKey(application => application.Id);
        builder.Property(application => application.TenantId).HasMaxLength(200).IsRequired();
        builder.Property(application => application.Cnpj).HasMaxLength(14).IsRequired();
        builder.HasIndex(application => new { application.TenantId, application.Cnpj }).IsUnique();
        builder.Property(application => application.CorporateName).HasMaxLength(500).IsRequired();
        builder.Property(application => application.InstitutionalEmail).HasMaxLength(320).IsRequired();
        builder.Property(application => application.DossierId).HasMaxLength(200).IsRequired();
        builder.Property(application => application.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.HasMany(application => application.Responsibles).WithOne().HasForeignKey(link => link.LegalEntityMembershipApplicationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class LegalEntityResponsibleConfiguration : IEntityTypeConfiguration<LegalEntityResponsible>
{
    public void Configure(EntityTypeBuilder<LegalEntityResponsible> builder)
    {
        builder.ToTable("LegalEntityResponsibles");
        builder.HasKey(link => new { link.LegalEntityMembershipApplicationId, link.ResponsiblePersonId });
        builder.HasOne(link => link.ResponsiblePerson).WithMany(person => person.LegalEntityApplications).HasForeignKey(link => link.ResponsiblePersonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(link => link.LegalEntityMembershipApplicationId)
            .HasFilter("\"IsPrincipal\" = true AND \"IsActive\" = true")
            .IsUnique();
    }
}

public sealed class ResponsibleUserLinkConfiguration : IEntityTypeConfiguration<ResponsibleUserLink>
{
    public void Configure(EntityTypeBuilder<ResponsibleUserLink> builder)
    {
        builder.ToTable("ResponsibleUserLinks");
        builder.HasKey(link => new { link.ResponsiblePersonId, link.UserId });
        builder.Property(link => link.UserId).HasMaxLength(450).IsRequired();
        builder.HasOne(link => link.ResponsiblePerson).WithMany().HasForeignKey(link => link.ResponsiblePersonId).OnDelete(DeleteBehavior.Restrict);
    }
}