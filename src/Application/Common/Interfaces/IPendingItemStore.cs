using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.Common.Interfaces;

public interface IPendingItemStore
{
    Task AddAsync(PendingItem pendingItem, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
