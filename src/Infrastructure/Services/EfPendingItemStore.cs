using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Data;

namespace Capri.Sgr.Infrastructure.Services;

public sealed class EfPendingItemStore(ApplicationDbContext context) : IPendingItemStore
{
    public Task AddAsync(PendingItem pendingItem, CancellationToken cancellationToken)
        => context.PendingItems.AddAsync(pendingItem, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => context.SaveChangesAsync(cancellationToken);
}
