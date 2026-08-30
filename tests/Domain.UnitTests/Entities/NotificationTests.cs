using Capri.Sgr.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Domain.UnitTests.Entities;

public class NotificationTests
{
    [Test]
    public void FailedNotificationCanBeResentAndItsDeliveryHistoryIsPreserved()
    {
        var requestedAt = new DateTimeOffset(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
        var notification = new Notification("Pendência aberta", "associado@example.test", "email", "Regularize a cobrança", requestedAt);

        notification.MarkFailed(requestedAt.AddMinutes(1), "SMTP unavailable");
        notification.Resend();
        notification.MarkSent(requestedAt.AddMinutes(2));

        notification.Status.ShouldBe(NotificationStatus.Sent);
        notification.AttemptCount.ShouldBe(2);
        notification.Failure.ShouldBeNull();
        notification.DeliveredAt.ShouldBe(requestedAt.AddMinutes(2));
    }

    [Test]
    public void SentNotificationCannotBeMarkedAsFailed()
    {
        var notification = new Notification("Pendência aberta", "associado@example.test", "email", "Regularize a cobrança", DateTimeOffset.UtcNow);
        notification.MarkSent(DateTimeOffset.UtcNow);

        Should.Throw<InvalidOperationException>(() => notification.MarkFailed(DateTimeOffset.UtcNow, "SMTP unavailable"));
    }
}
