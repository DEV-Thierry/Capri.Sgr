using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Application.Notifications;
using Capri.Sgr.Domain.Entities;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Application.UnitTests.Notifications;

public class NotificationDeliveryServiceTests
{
    [Test]
    public async Task DeliveryFailureIsRecordedWithoutPropagatingToBusinessCaller()
    {
        var store = new Mock<INotificationStore>();
        var sender = new Mock<INotificationSender>();
        var clock = new Mock<IOperationalClock>();
        var now = new DateTimeOffset(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
        clock.Setup(item => item.GetCurrentInstant()).Returns(now);
        sender.Setup(item => item.SendAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SMTP unavailable"));
        var notification = new Notification("Pendência aberta", "associado@example.test", "email", "Regularize a cobrança", now);
        var service = new NotificationDeliveryService(store.Object, sender.Object, clock.Object);

        await Should.NotThrowAsync(() => service.DeliverAsync(notification, CancellationToken.None));

        notification.Status.ShouldBe(NotificationStatus.Failed);
        notification.Failure.ShouldBe("SMTP unavailable");
        store.Verify(item => item.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task ResendDeliversPreviouslyFailedNotification()
    {
        var store = new Mock<INotificationStore>();
        var sender = new Mock<INotificationSender>();
        var clock = new Mock<IOperationalClock>();
        var now = new DateTimeOffset(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
        clock.Setup(item => item.GetCurrentInstant()).Returns(now);
        var notification = new Notification("Pendência aberta", "associado@example.test", "email", "Regularize a cobrança", now);
        notification.MarkFailed(now, "SMTP unavailable");
        var service = new NotificationDeliveryService(store.Object, sender.Object, clock.Object);

        await service.ResendAsync(notification, CancellationToken.None);

        notification.Status.ShouldBe(NotificationStatus.Sent);
        notification.AttemptCount.ShouldBe(2);
        store.Verify(item => item.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}
