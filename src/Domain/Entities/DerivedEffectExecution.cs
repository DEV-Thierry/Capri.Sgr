namespace Capri.Sgr.Domain.Entities;

public enum DerivedEffectExecutionStatus
{
    InProgress,
    Succeeded,
    Failed
}

/// <summary>
/// Durable idempotency record for a derived effect. One execution key represents one logical effect.
/// </summary>
public sealed class DerivedEffectExecution
{
    private DerivedEffectExecution() { }

    public DerivedEffectExecution(string executionKey, DateTimeOffset startedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executionKey);

        Id = Guid.NewGuid();
        ExecutionKey = executionKey;
        StartedAt = startedAt;
        Status = DerivedEffectExecutionStatus.InProgress;
    }

    public Guid Id { get; private set; }
    public string ExecutionKey { get; private set; } = null!;
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? FinishedAt { get; private set; }
    public DerivedEffectExecutionStatus Status { get; private set; }
    public string? Failure { get; private set; }

    public void MarkSucceeded(DateTimeOffset finishedAt)
    {
        if (Status == DerivedEffectExecutionStatus.Succeeded)
            return;

        Status = DerivedEffectExecutionStatus.Succeeded;
        FinishedAt = finishedAt;
        Failure = null;
    }

    public void MarkFailed(DateTimeOffset finishedAt, string failure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(failure);
        if (Status == DerivedEffectExecutionStatus.Succeeded)
            throw new InvalidOperationException("A successful derived effect cannot be marked as failed.");

        Status = DerivedEffectExecutionStatus.Failed;
        FinishedAt = finishedAt;
        Failure = failure;
    }

    public void Retry(DateTimeOffset startedAt)
    {
        if (Status != DerivedEffectExecutionStatus.Failed)
            throw new InvalidOperationException("Only failed derived effects can be retried.");

        Status = DerivedEffectExecutionStatus.InProgress;
        StartedAt = startedAt;
        FinishedAt = null;
        Failure = null;
    }
}
