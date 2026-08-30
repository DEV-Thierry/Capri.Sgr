using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    DbSet<AuditRecord> AuditRecords { get; }

    DbSet<DerivedEffectExecution> DerivedEffectExecutions { get; }

    DbSet<PendingItem> PendingItems { get; }

    DbSet<Notification> Notifications { get; }

    DbSet<AssociatedDocument> AssociatedDocuments { get; }

    DbSet<Charge> Charges { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
