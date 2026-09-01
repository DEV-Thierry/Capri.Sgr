using Capri.Sgr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capri.Sgr.Infrastructure.Data.Configurations;

public sealed class MembershipApplicationConfiguration : IEntityTypeConfiguration<MembershipApplication>
{
    public void Configure(EntityTypeBuilder<MembershipApplication> builder)
    {
        builder.ToTable("MembershipApplications");
        builder.HasKey(application => application.Id);
        builder.Property(application => application.TenantId).HasMaxLength(200).IsRequired();
        builder.Property(application => application.ApplicantId).HasMaxLength(450);
        builder.Property(application => application.Cpf).HasMaxLength(11);
        builder.HasIndex(application => new { application.TenantId, application.Cpf }).IsUnique();
        builder.Property(application => application.FullName).HasMaxLength(500);
        builder.Property(application => application.Email).HasMaxLength(500);
        builder.Property(application => application.PrimaryPhone).HasMaxLength(100);
        builder.Property(application => application.CorrespondenceAddress).HasMaxLength(2000);
        builder.Property(application => application.IdentityDocument).HasMaxLength(500);
        builder.Property(application => application.AcceptedTermVersion).HasMaxLength(200);
        builder.Property(application => application.TermAcceptanceActorId).HasMaxLength(450);
        builder.Property(application => application.TermAcceptanceChannel).HasMaxLength(100);
        builder.Property(application => application.Status).HasConversion<string>().HasMaxLength(64).IsRequired();
    }
}
