using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Data;

namespace Capri.Sgr.Infrastructure.Services;

public sealed class EfNotificationStore(ApplicationDbContext context) : INotificationStore
{
    public Task AddAsync(Notification notification, CancellationToken cancellationToken)
        => context.Notifications.AddAsync(notification, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => context.SaveChangesAsync(cancellationToken);
}
