namespace Capri.Sgr.Domain.Entities;

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed
}

/// <summary>
/// Traceable delivery request. Delivery failure is recorded here and does not roll
/// back the business transition which requested the notification.
/// </summary>
public sealed class Notification
{
    private Notification() { }

    public Notification(string eventType, string recipient, string channel, string body, DateTimeOffset requestedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        ArgumentException.ThrowIfNullOrWhiteSpace(recipient);
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);

        Id = Guid.NewGuid();
        EventType = eventType;
        Recipient = recipient;
        Channel = channel;
        Body = body;
        RequestedAt = requestedAt;
        Status = NotificationStatus.Pending;
    }

    public Guid Id { get; private set; }
    public string EventType { get; private set; } = null!;
    public string Recipient { get; private set; } = null!;
    public string Channel { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public DateTimeOffset RequestedAt { get; private set; }
    public DateTimeOffset? LastAttemptAt { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; }
    public int AttemptCount { get; private set; }
    public string? Failure { get; private set; }
    public NotificationStatus Status { get; private set; }

    public void MarkSent(DateTimeOffset deliveredAt)
    {
        if (Status == NotificationStatus.Sent)
            return;

        AttemptCount++;
        LastAttemptAt = deliveredAt;
        DeliveredAt = deliveredAt;
        Failure = null;
        Status = NotificationStatus.Sent;
    }

    public void MarkFailed(DateTimeOffset attemptedAt, string failure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(failure);
        if (Status == NotificationStatus.Sent)
            throw new InvalidOperationException("A sent notification cannot be marked as failed.");

        AttemptCount++;
        LastAttemptAt = attemptedAt;
        Failure = failure;
        Status = NotificationStatus.Failed;
    }

    public void Resend()
    {
        if (Status != NotificationStatus.Failed)
            throw new InvalidOperationException("Only failed notifications can be resent.");

        Status = NotificationStatus.Pending;
        Failure = null;
    }
}
