namespace Capri.Sgr.Domain.Entities;

public enum PendingItemStatus
{
    Open,
    Resolved,
    Cancelled
}

/// <summary>
/// A business obligation with an independent lifecycle. It never changes the primary
/// status of the resource that originated it.
/// </summary>
public sealed class PendingItem
{
    private PendingItem() { }

    public PendingItem(string cause, string impact, string regularization, DateTimeOffset openedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cause);
        ArgumentException.ThrowIfNullOrWhiteSpace(impact);
        ArgumentException.ThrowIfNullOrWhiteSpace(regularization);

        Id = Guid.NewGuid();
        Cause = cause;
        Impact = impact;
        Regularization = regularization;
        OpenedAt = openedAt;
        Status = PendingItemStatus.Open;
    }

    public Guid Id { get; private set; }
    public string Cause { get; private set; } = null!;
    public string Impact { get; private set; } = null!;
    public string Regularization { get; private set; } = null!;
    public DateTimeOffset OpenedAt { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public string? ClosureReason { get; private set; }
    public PendingItemStatus Status { get; private set; }

    public void Resolve(DateTimeOffset resolvedAt, string resolution)
        => Close(PendingItemStatus.Resolved, resolvedAt, resolution);

    public void Cancel(DateTimeOffset cancelledAt, string reason)
        => Close(PendingItemStatus.Cancelled, cancelledAt, reason);

    private void Close(PendingItemStatus status, DateTimeOffset closedAt, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        if (Status != PendingItemStatus.Open)
            throw new InvalidOperationException("Only open pending items can be closed.");

        Status = status;
        ClosedAt = closedAt;
        ClosureReason = reason;
    }
}
