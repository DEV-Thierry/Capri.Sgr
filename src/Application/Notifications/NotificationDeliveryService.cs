using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.Notifications;

/// <summary>
/// Records a delivery attempt without propagating delivery failures to the caller's
/// business transition. Failed records remain available for explicit resend.
/// </summary>
public sealed class NotificationDeliveryService(INotificationStore store, INotificationSender sender, IOperationalClock clock)
{
    public async Task DeliverAsync(Notification notification, CancellationToken cancellationToken)
    {
        try
        {
            await sender.SendAsync(notification, cancellationToken);
            notification.MarkSent(clock.GetCurrentInstant());
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            notification.MarkFailed(clock.GetCurrentInstant(), exception.Message);
        }

        await store.SaveChangesAsync(cancellationToken);
    }

    public async Task ResendAsync(Notification notification, CancellationToken cancellationToken)
    {
        notification.Resend();
        await DeliverAsync(notification, cancellationToken);
    }
}
