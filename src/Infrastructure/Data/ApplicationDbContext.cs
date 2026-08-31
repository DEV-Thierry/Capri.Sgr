using System.Reflection;
using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Capri.Sgr.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();

    public DbSet<DerivedEffectExecution> DerivedEffectExecutions => Set<DerivedEffectExecution>();

    public DbSet<PendingItem> PendingItems => Set<PendingItem>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<AssociatedDocument> AssociatedDocuments => Set<AssociatedDocument>();

    public DbSet<Charge> Charges => Set<Charge>();

    public DbSet<ResponsiblePerson> ResponsiblePeople => Set<ResponsiblePerson>();

    public DbSet<LegalEntityMembershipApplication> LegalEntityMembershipApplications => Set<LegalEntityMembershipApplication>();

    public DbSet<MembershipApplication> MembershipApplications => Set<MembershipApplication>();

    public DbSet<ResponsibleUserLink> ResponsibleUserLinks => Set<ResponsibleUserLink>();

    public DbSet<AssociateMembership> AssociateMemberships => Set<AssociateMembership>();

    public DbSet<PrefixProposal> PrefixProposals => Set<PrefixProposal>();

    public DbSet<ReclassificationProposal> ReclassificationProposals => Set<ReclassificationProposal>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
