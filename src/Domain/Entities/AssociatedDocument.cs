namespace Capri.Sgr.Domain.Entities;

/// <summary>
/// Preserved dossier of a formal document required by another bounded context.
/// The owner reference is deliberately opaque so this capability does not own
/// Solicitação de associação or Associado workflows.
/// </summary>
public sealed class AssociatedDocument
{
    private readonly List<AssociatedDocumentVersion> _versions = [];

    private AssociatedDocument() { }

    public AssociatedDocument(string dossierId, string documentType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dossierId);
        ArgumentException.ThrowIfNullOrWhiteSpace(documentType);

        Id = Guid.NewGuid();
        DossierId = dossierId;
        DocumentType = documentType;
    }

    public Guid Id { get; private set; }
    public string DossierId { get; private set; } = null!;
    public string DocumentType { get; private set; } = null!;
    public IReadOnlyCollection<AssociatedDocumentVersion> Versions => _versions.AsReadOnly();
    public AssociatedDocumentVersion? CurrentVersion => _versions.SingleOrDefault(version => version.IsCurrent);

    /// <summary>Creates an untrusted version. It cannot participate in validation yet.</summary>
    public AssociatedDocumentVersion AddVersion(
        string authorId,
        DateTimeOffset submittedAt,
        string originalFileName,
        string contentType,
        long length,
        string contentHash,
        string? reason = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(authorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(originalFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentHash);

        return AddVersion(new AssociatedDocumentVersion(
            _versions.Count + 1,
            authorId,
            submittedAt,
            originalFileName,
            contentType,
            length,
            contentHash,
            reason));
    }

    public void ConfirmSecureStorageAndAudit(Guid versionId, string storageReference, Guid auditRecordId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(storageReference);
        var version = FindVersion(versionId);
        if (version.Status != AssociatedDocumentVersionStatus.PendingSecureStorage)
            throw new InvalidOperationException("Only a pending document version can be confirmed.");

        CurrentVersion?.Supersede();
        version.ConfirmSecureStorageAndAudit(storageReference, auditRecordId);
    }

    public void StartAnalysis(Guid versionId, string analystId, DateTimeOffset analysedAt)
    {
        FindCurrentVersion(versionId).StartAnalysis(analystId, analysedAt);
    }

    public void Approve(Guid versionId, string analystId, DateTimeOffset analysedAt, string reason)
    {
        FindCurrentVersion(versionId).Approve(analystId, analysedAt, reason);
    }

    public void Reject(Guid versionId, string analystId, DateTimeOffset analysedAt, string reason)
    {
        FindCurrentVersion(versionId).Reject(analystId, analysedAt, reason);
    }

    public void RequestResubmission(Guid versionId, string analystId, DateTimeOffset analysedAt, string reason)
    {
        FindCurrentVersion(versionId).RequestResubmission(analystId, analysedAt, reason);
    }

    public bool HasApprovedCurrentVersion() => CurrentVersion?.Status == AssociatedDocumentVersionStatus.Approved;

    private AssociatedDocumentVersion AddVersion(AssociatedDocumentVersion version)
    {
        _versions.Add(version);
        return version;
    }

    private AssociatedDocumentVersion FindVersion(Guid versionId) =>
        _versions.SingleOrDefault(version => version.Id == versionId)
        ?? throw new InvalidOperationException("The document version does not belong to this dossier.");

    private AssociatedDocumentVersion FindCurrentVersion(Guid versionId)
    {
        var version = FindVersion(versionId);
        if (!version.IsCurrent)
            throw new InvalidOperationException("Only the current document version can be analysed.");

        return version;
    }
}

public sealed class AssociatedDocumentVersion
{
    private AssociatedDocumentVersion() { }

    internal AssociatedDocumentVersion(
        int number,
        string authorId,
        DateTimeOffset submittedAt,
        string originalFileName,
        string contentType,
        long length,
        string contentHash,
        string? reason)
    {
        Id = Guid.NewGuid();
        Number = number;
        AuthorId = authorId;
        SubmittedAt = submittedAt;
        OriginalFileName = originalFileName;
        ContentType = contentType;
        Length = length;
        ContentHash = contentHash;
        Reason = reason;
        Status = AssociatedDocumentVersionStatus.PendingSecureStorage;
        IsCurrent = false;
    }

    public Guid Id { get; private set; }
    public int Number { get; private set; }
    public string AuthorId { get; private set; } = null!;
    public DateTimeOffset SubmittedAt { get; private set; }
    public string OriginalFileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long Length { get; private set; }
    public string ContentHash { get; private set; } = null!;
    public string? StorageReference { get; private set; }
    public Guid? StorageAuditRecordId { get; private set; }
    public AssociatedDocumentVersionStatus Status { get; private set; }
    public string? Reason { get; private set; }
    public string? DecisionAuthorId { get; private set; }
    public DateTimeOffset? DecidedAt { get; private set; }
    public bool IsCurrent { get; private set; }

    /// <summary>True only after secure storage and Auditoria are both confirmed.</summary>
    public bool IsSecurelyStored => Status != AssociatedDocumentVersionStatus.PendingSecureStorage &&
                                    StorageReference is not null && StorageAuditRecordId is not null;

    public bool CanParticipateInValidation => IsCurrent && Status == AssociatedDocumentVersionStatus.Approved;

    internal void ConfirmSecureStorageAndAudit(string storageReference, Guid auditRecordId)
    {
        StorageReference = storageReference;
        StorageAuditRecordId = auditRecordId;
        IsCurrent = true;
        Status = AssociatedDocumentVersionStatus.Sent;
    }

    internal void StartAnalysis(string analystId, DateTimeOffset analysedAt)
    {
        EnsureSecurelyStored();
        EnsureNonTerminal();
        DecisionAuthorId = analystId;
        DecidedAt = analysedAt;
        Status = AssociatedDocumentVersionStatus.InAnalysis;
    }

    internal void Approve(string analystId, DateTimeOffset analysedAt, string reason) =>
        Decide(AssociatedDocumentVersionStatus.Approved, analystId, analysedAt, reason);

    internal void Reject(string analystId, DateTimeOffset analysedAt, string reason) =>
        Decide(AssociatedDocumentVersionStatus.Rejected, analystId, analysedAt, reason);

    internal void RequestResubmission(string analystId, DateTimeOffset analysedAt, string reason) =>
        Decide(AssociatedDocumentVersionStatus.ReturnedForResubmission, analystId, analysedAt, reason);

    internal void Supersede()
    {
        if (Status == AssociatedDocumentVersionStatus.PendingSecureStorage)
            throw new InvalidOperationException("An unconfirmed document version cannot supersede the current version.");

        IsCurrent = false;
        Status = AssociatedDocumentVersionStatus.Superseded;
    }

    private void Decide(AssociatedDocumentVersionStatus status, string analystId, DateTimeOffset analysedAt, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(analystId);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        EnsureSecurelyStored();
        if (Status is not (AssociatedDocumentVersionStatus.Sent or AssociatedDocumentVersionStatus.InAnalysis))
            throw new InvalidOperationException("Only a sent or in-analysis document version can receive a decision.");

        DecisionAuthorId = analystId;
        DecidedAt = analysedAt;
        Reason = reason;
        Status = status;
    }

    private void EnsureSecurelyStored()
    {
        if (!IsSecurelyStored)
            throw new InvalidOperationException("The document version is not valid until secure storage and Auditoria are confirmed.");
    }

    private void EnsureNonTerminal()
    {
        if (Status is not AssociatedDocumentVersionStatus.Sent)
            throw new InvalidOperationException("Only a sent document version can enter analysis.");
    }
}

public enum AssociatedDocumentVersionStatus
{
    PendingSecureStorage,
    Sent,
    InAnalysis,
    Approved,
    Rejected,
    ReturnedForResubmission,
    Superseded
}
