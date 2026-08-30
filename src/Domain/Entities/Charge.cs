namespace Capri.Sgr.Domain.Entities;

public enum ChargeStatus
{
    Open,
    Paid,
    Cancelled,
    Refunded
}

/// <summary>Immutable reference to a business fact that generated a Cobrança.</summary>
public sealed record ChargeGeneratorFact
{
    public ChargeGeneratorFact(string sourceType, string sourceId, string kind)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(kind);
        SourceType = sourceType;
        SourceId = sourceId;
        Kind = kind;
    }

    public string SourceType { get; }
    public string SourceId { get; }
    public string Kind { get; }
}

/// <summary>Evidence retained for a manual financial decision.</summary>
public sealed record ChargeDecision
{
    public ChargeDecision(string reason, string evidence, DateTimeOffset decidedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        ArgumentException.ThrowIfNullOrWhiteSpace(evidence);
        Reason = reason;
        Evidence = evidence;
        DecidedAt = decidedAt;
    }

    public string Reason { get; }
    public string Evidence { get; }
    public DateTimeOffset DecidedAt { get; }
}

/// <summary>Evidence retained for one manual payment recording.</summary>
public sealed record ChargePayment
{
    public ChargePayment(string reference, string evidence, DateTimeOffset recordedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reference);
        ArgumentException.ThrowIfNullOrWhiteSpace(evidence);
        Reference = reference;
        Evidence = evidence;
        RecordedAt = recordedAt;
    }

    public string Reference { get; }
    public string Evidence { get; }
    public DateTimeOffset RecordedAt { get; }
}

/// <summary>Request to create a financial Pendência for a due Cobrança.</summary>
public sealed record FinancialPending(Guid ChargeId, string Cause, string Description);

/// <summary>Event seam for notifying the Pagador that a Cobrança is due.</summary>
public sealed record ChargeDueNotification(Guid ChargeId, string PayerId, DateTimeOffset DueAt);

/// <summary>Derived due-date effects; delivery and Pendência lifecycle belong to their owning modules.</summary>
public sealed record ChargeDueEffect(FinancialPending Pending, ChargeDueNotification Notification);

/// <summary>
/// Financial obligation linked to one or more business facts. Its lifecycle deliberately remains
/// independent from Pendências and notifications generated after its due date.
/// </summary>
public sealed class Charge
{
    private readonly List<ChargeGeneratorFact> _generatorFacts = [];

    private Charge() { }

    private Charge(string payerId, decimal amount, DateTimeOffset dueAt, IEnumerable<ChargeGeneratorFact> generatorFacts, DateTimeOffset issuedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(payerId);
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Charge amount must be positive.");

        var facts = generatorFacts?.ToArray() ?? throw new ArgumentNullException(nameof(generatorFacts));
        if (facts.Length == 0)
            throw new ArgumentException("At least one generator fact is required.", nameof(generatorFacts));
        if (facts.Distinct().Count() != facts.Length)
            throw new ArgumentException("Generator facts must be unique.", nameof(generatorFacts));

        Id = Guid.NewGuid();
        PayerId = payerId;
        Amount = amount;
        DueAt = dueAt;
        IssuedAt = issuedAt;
        Status = ChargeStatus.Open;
        _generatorFacts.AddRange(facts);
    }

    public Guid Id { get; private set; }
    public string PayerId { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public DateTimeOffset IssuedAt { get; private set; }
    public DateTimeOffset DueAt { get; private set; }
    public ChargeStatus Status { get; private set; }
    public IReadOnlyCollection<ChargeGeneratorFact> GeneratorFacts => _generatorFacts.AsReadOnly();
    public ChargePayment? Payment { get; private set; }
    public ChargeDecision? Cancellation { get; private set; }
    public ChargeDecision? RefundDecision { get; private set; }

    public static Charge Create(string payerId, decimal amount, DateTimeOffset dueAt, IEnumerable<ChargeGeneratorFact> generatorFacts, DateTimeOffset issuedAt) =>
        new(payerId, amount, dueAt, generatorFacts, issuedAt);

    public void RecordPayment(string reference, string evidence, DateTimeOffset recordedAt)
    {
        EnsureStatus(ChargeStatus.Open, "Only an open charge can receive a payment.");
        Payment = new ChargePayment(reference, evidence, recordedAt);
        Status = ChargeStatus.Paid;
    }

    public void Cancel(string reason, string evidence, DateTimeOffset decidedAt)
    {
        EnsureStatus(ChargeStatus.Open, "Only an open charge can be cancelled.");
        Cancellation = new ChargeDecision(reason, evidence, decidedAt);
        Status = ChargeStatus.Cancelled;
    }

    public void Refund(string reason, string evidence, DateTimeOffset decidedAt)
    {
        EnsureStatus(ChargeStatus.Paid, "Only a paid charge can be refunded.");
        RefundDecision = new ChargeDecision(reason, evidence, decidedAt);
        Status = ChargeStatus.Refunded;
    }

    public ChargeDueEffect? GetDueEffect(DateTimeOffset occurredAt)
    {
        if (Status != ChargeStatus.Open || occurredAt <= DueAt)
            return null;

        return new ChargeDueEffect(
            new FinancialPending(Id, "Vencimento", "Cobrança vencida"),
            new ChargeDueNotification(Id, PayerId, DueAt));
    }

    private void EnsureStatus(ChargeStatus expectedStatus, string message)
    {
        if (Status != expectedStatus)
            throw new InvalidOperationException(message);
    }
}
