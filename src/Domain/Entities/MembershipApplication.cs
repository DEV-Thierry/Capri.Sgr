using Capri.Sgr.Domain.ValueObjects;

namespace Capri.Sgr.Domain.Entities;

public enum MembershipApplicationStatus
{
    Draft,
    AwaitingDocuments,
    AwaitingPayment,
    AwaitingApproval
}

public enum ProvisionalMembershipPermission
{
    FollowUp,
    CorrectReleasedInformation,
    ManageDocuments,
    PayCharges,
    OperationalRequests,
    AnimalManagement,
    Services
}

/// <summary>
/// A PF Solicitação de associação. Approval decisions intentionally belong to a later workflow.
/// </summary>
public sealed class MembershipApplication
{
    private MembershipApplication() { }

    public MembershipApplication(string tenantId, string? applicantId, DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        Id = Guid.NewGuid();
        TenantId = tenantId;
        ApplicantId = applicantId;
        CreatedAt = createdAt;
        Status = MembershipApplicationStatus.Draft;
    }

    public Guid Id { get; private set; }
    public string TenantId { get; private set; } = null!;
    public string? ApplicantId { get; private set; }
    public string? Cpf { get; private set; }
    public string? FullName { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public string? Email { get; private set; }
    public string? PrimaryPhone { get; private set; }
    public string? CorrespondenceAddress { get; private set; }
    public string? IdentityDocument { get; private set; }
    public bool TermAccepted { get; private set; }
    public string? AcceptedTermVersion { get; private set; }
    public string? TermAcceptanceActorId { get; private set; }
    public DateTimeOffset? TermAcceptedAt { get; private set; }
    public string? TermAcceptanceChannel { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public MembershipApplicationStatus Status { get; private set; }
    public Guid? InitialChargeId { get; private set; }

    public string DossierId => $"membership-application:{Id}";
    public bool HasProvisionalAccess => Status is not MembershipApplicationStatus.Draft;

    public void UpdateIdentityAndContact(
        string cpf, string fullName, DateOnly birthDate, string email, string primaryPhone,
        string correspondenceAddress, string identityDocument)
    {
        EnsureDraft();
        var normalizedCpf = new Cpf(cpf);
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(primaryPhone) || string.IsNullOrWhiteSpace(correspondenceAddress) ||
            string.IsNullOrWhiteSpace(identityDocument))
            throw new ArgumentException("PF identity and contact fields are required.");
        if (!email.Contains('@', StringComparison.Ordinal) || birthDate >= DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("PF identity and contact fields are invalid.");

        Cpf = normalizedCpf.Value;
        FullName = fullName.Trim();
        BirthDate = birthDate;
        Email = email.Trim();
        PrimaryPhone = primaryPhone.Trim();
        CorrespondenceAddress = correspondenceAddress.Trim();
        IdentityDocument = identityDocument.Trim();
    }

    public void AcceptTerm(string version, string actorId, string channel, DateTimeOffset acceptedAt)
    {
        EnsureDraft();
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(actorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        TermAccepted = true;
        AcceptedTermVersion = version;
        TermAcceptanceActorId = actorId;
        TermAcceptanceChannel = channel;
        TermAcceptedAt = acceptedAt;
    }

    public void Submit(DateTimeOffset submittedAt, Guid? initialChargeId)
    {
        EnsureDraft();
        if (Cpf is null || FullName is null || BirthDate is null || Email is null || PrimaryPhone is null ||
            CorrespondenceAddress is null || IdentityDocument is null)
            throw new InvalidOperationException("PF identity and contact data must be completed before submission.");
        if (!TermAccepted)
            throw new InvalidOperationException("Explicit term acceptance is required before submission.");

        InitialChargeId = initialChargeId;
        SubmittedAt = submittedAt;
        Status = initialChargeId is null ? MembershipApplicationStatus.AwaitingDocuments : MembershipApplicationStatus.AwaitingPayment;
    }

    public bool HasPermission(ProvisionalMembershipPermission permission) => permission switch
    {
        ProvisionalMembershipPermission.FollowUp or
        ProvisionalMembershipPermission.CorrectReleasedInformation or
        ProvisionalMembershipPermission.ManageDocuments or
        ProvisionalMembershipPermission.PayCharges => HasProvisionalAccess,
        _ => false
    };

    private void EnsureDraft()
    {
        if (Status != MembershipApplicationStatus.Draft)
            throw new InvalidOperationException("Only a draft membership application can be changed.");
    }
}
