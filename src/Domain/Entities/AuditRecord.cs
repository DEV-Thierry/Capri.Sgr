namespace Capri.Sgr.Domain.Entities;

/// <summary>Immutable record of a relevant business fact.</summary>
public sealed class AuditRecord
{
    private AuditRecord() { }

    public AuditRecord(
        string? actorId,
        DateTimeOffset occurredAt,
        string channel,
        string entityType,
        string entityId,
        string action,
        string? reason = null,
        string? before = null,
        string? after = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        ArgumentException.ThrowIfNullOrWhiteSpace(entityType);
        ArgumentException.ThrowIfNullOrWhiteSpace(entityId);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        Id = Guid.NewGuid();
        ActorId = actorId;
        OccurredAt = occurredAt;
        Channel = channel;
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        Reason = reason;
        Before = before;
        After = after;
    }

    public Guid Id { get; private set; }
    public string? ActorId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string Channel { get; private set; } = null!;
    public string EntityType { get; private set; } = null!;
    public string EntityId { get; private set; } = null!;
    public string Action { get; private set; } = null!;
    public string? Reason { get; private set; }
    public string? Before { get; private set; }
    public string? After { get; private set; }
}
