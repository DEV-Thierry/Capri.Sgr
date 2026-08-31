using System.Globalization;
using System.Text;

namespace Capri.Sgr.Domain.Entities;

public enum AssociatePersonKind { NaturalPerson, LegalEntity }

public enum AssociateType
{
    ContributingJunior,
    ContributingSenior,
    Young,
    NonMemberBreeder,
    LifeMember,
    User
}

public enum MembershipChargeKind { Quarterly, Annual, OneTime, None }

public sealed record AssociateTypeRule(
    AssociateType Type,
    bool AllowsNaturalPerson,
    bool AllowsLegalEntity,
    int MinimumActiveHerd,
    int? MaximumActiveHerd,
    MembershipChargeKind ChargeKind,
    decimal? ChargeAmount,
    decimal DiscountRate,
    bool AllowsPrefix);

/// <summary>Closed, code-owned catalogue. It is deliberately not persisted or editable at runtime.</summary>
public static class AssociateTypeCatalogue
{
    private static readonly IReadOnlyDictionary<AssociateType, AssociateTypeRule> Rules =
        new Dictionary<AssociateType, AssociateTypeRule>
        {
            [AssociateType.ContributingJunior] = new(AssociateType.ContributingJunior, true, true, 6, 59, MembershipChargeKind.Quarterly, 396m, .50m, true),
            [AssociateType.ContributingSenior] = new(AssociateType.ContributingSenior, true, true, 60, null, MembershipChargeKind.Quarterly, 3150m, .50m, true),
            [AssociateType.Young] = new(AssociateType.Young, true, false, 1, null, MembershipChargeKind.Quarterly, 193m, .50m, true),
            [AssociateType.NonMemberBreeder] = new(AssociateType.NonMemberBreeder, true, false, 1, null, MembershipChargeKind.None, null, 0m, false),
            [AssociateType.LifeMember] = new(AssociateType.LifeMember, true, true, 1, null, MembershipChargeKind.OneTime, null, .50m, true),
            [AssociateType.User] = new(AssociateType.User, true, false, 0, 5, MembershipChargeKind.Annual, 209m, .50m, false)
        };

    public static AssociateTypeRule Get(AssociateType type) => Rules[type];

    public static bool IsEligible(AssociateType type, AssociatePersonKind personKind)
    {
        var rule = Get(type);
        return personKind == AssociatePersonKind.NaturalPerson ? rule.AllowsNaturalPerson : rule.AllowsLegalEntity;
    }
}

public static class PrefixNormalizer
{
    public static string Normalize(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        var decomposed = value.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (var character in decomposed)
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                builder.Append(character);
        return builder.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
    }
}

public enum PrefixKind { Prefix, Suffix }
public enum PrefixProposalStatus { Open, Approved, Rejected, Cancelled }

/// <summary>A proposed change; its approval is the only operation that changes the current Afixo.</summary>
public sealed class PrefixProposal
{
    private PrefixProposal() { }
    public PrefixProposal(Guid membershipId, PrefixKind? kind, string? displayValue, DateTimeOffset proposedAt)
    {
        if ((kind is null) != (displayValue is null))
            throw new ArgumentException("Afixo kind and value must be supplied together, or both omitted for removal.");
        Id = Guid.NewGuid(); MembershipId = membershipId; Kind = kind; DisplayValue = displayValue?.Trim();
        NormalizedValue = displayValue is null ? null : PrefixNormalizer.Normalize(displayValue); ProposedAt = proposedAt; Status = PrefixProposalStatus.Open;
    }
    public Guid Id { get; private set; }
    public Guid MembershipId { get; private set; }
    public PrefixKind? Kind { get; private set; }
    public string? DisplayValue { get; private set; }
    public string? NormalizedValue { get; private set; }
    public DateTimeOffset ProposedAt { get; private set; }
    public PrefixProposalStatus Status { get; private set; }
    public void Approve() => Status = PrefixProposalStatus.Approved;
    public void Reject() => Status = PrefixProposalStatus.Rejected;
    public void Cancel() => Status = PrefixProposalStatus.Cancelled;
}

public enum ReclassificationProposalStatus { Open, Cancelled, Decided }
public sealed class ReclassificationProposal
{
    private ReclassificationProposal() { }
    public ReclassificationProposal(Guid membershipId, AssociateType? proposedType, int activeHerdCount, DateTimeOffset evaluatedAt)
    { Id = Guid.NewGuid(); MembershipId = membershipId; ProposedType = proposedType; ActiveHerdCount = activeHerdCount; EvaluatedAt = evaluatedAt; Status = ReclassificationProposalStatus.Open; }
    public Guid Id { get; private set; }
    public Guid MembershipId { get; private set; }
    public AssociateType? ProposedType { get; private set; }
    public int ActiveHerdCount { get; private set; }
    public DateTimeOffset EvaluatedAt { get; private set; }
    public ReclassificationProposalStatus Status { get; private set; }
    public void Update(AssociateType? proposedType, int activeHerdCount, DateTimeOffset evaluatedAt) { ProposedType = proposedType; ActiveHerdCount = activeHerdCount; EvaluatedAt = evaluatedAt; }
    public void Cancel() => Status = ReclassificationProposalStatus.Cancelled;
}

/// <summary>Persisted associate membership slice; not an associate application or Animal model.</summary>
public sealed class AssociateMembership
{
    private AssociateMembership() { }
    public AssociateMembership(string associateId, AssociatePersonKind personKind, AssociateType type, DateOnly createdOn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(associateId);
        if (!AssociateTypeCatalogue.IsEligible(type, personKind)) throw new ArgumentException("Associate type is not eligible for this person kind.", nameof(type));
        Id = Guid.NewGuid(); AssociateId = associateId; PersonKind = personKind; Type = type; CreatedOn = createdOn;
    }
    public Guid Id { get; private set; }
    public string AssociateId { get; private set; } = null!;
    public AssociatePersonKind PersonKind { get; private set; }
    public AssociateType Type { get; private set; }
    public DateOnly CreatedOn { get; private set; }
    public PrefixKind? PrefixKind { get; private set; }
    public string? PrefixDisplayValue { get; private set; }
    public string? PrefixNormalizedValue { get; private set; }
    public void ApplyApprovedPrefix(PrefixProposal proposal)
    {
        if (proposal.MembershipId != Id || proposal.Status != PrefixProposalStatus.Approved) throw new InvalidOperationException("Only an approved proposal for this membership can change its Afixo.");
        PrefixKind = proposal.Kind; PrefixDisplayValue = proposal.DisplayValue; PrefixNormalizedValue = proposal.NormalizedValue;
    }
}