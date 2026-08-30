using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.Common.Interfaces;

/// <summary>Delivery boundary. Implementations may call e-mail or the internal notification centre.</summary>
public interface INotificationSender
{
    Task SendAsync(Notification notification, CancellationToken cancellationToken);
}
