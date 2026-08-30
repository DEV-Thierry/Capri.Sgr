using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.AssociatedDocuments;

/// <summary>
/// Coordinates the confirmation boundary: a version becomes usable only after
/// secure storage and its immutable audit evidence both succeed.
/// </summary>
public sealed class AssociatedDocumentSubmissionService(
    IAssociatedDocumentStore documents,
    ISecureDocumentStorage storage,
    IAuditStore auditStore,
    IAssociatedDocumentAccessAuthorizer accessAuthorizer)
{
    public async Task<AssociatedDocumentVersion> SubmitAsync(
        AssociatedDocumentSubmission request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await accessAuthorizer.DemandAsync(
            new AssociatedDocumentAccessRequest(request.AuthorId, request.DossierId, AssociatedDocumentAccessAction.SubmitVersion),
            cancellationToken);

        var document = request.DocumentId is { } documentId
            ? await documents.FindAsync(documentId, cancellationToken)
                ?? throw new InvalidOperationException("Associated document was not found.")
            : new AssociatedDocument(request.DossierId, request.DocumentType);

        if (document.DossierId != request.DossierId || document.DocumentType != request.DocumentType)
            throw new InvalidOperationException("The document does not belong to the supplied dossier and type.");

        var version = document.AddVersion(
            request.AuthorId,
            request.SubmittedAt,
            request.OriginalFileName,
            request.ContentType,
            request.Length,
            request.ContentHash,
            request.Reason);

        var stored = await storage.StoreAsync(
            new SecureDocumentStorageRequest(
                document.DossierId,
                version.OriginalFileName,
                version.ContentType,
                version.Length,
                version.ContentHash,
                request.Content),
            cancellationToken);

        var audit = new AuditRecord(
            request.AuthorId,
            request.SubmittedAt,
            request.Channel,
            nameof(AssociatedDocumentVersion),
            version.Id.ToString(),
            "SecureStorageConfirmed",
            request.Reason,
            after: $"{{\"documentId\":\"{document.Id}\",\"version\":{version.Number},\"storageReference\":\"{stored.StorageReference}\"}}");
        await auditStore.AppendAsync(audit, cancellationToken);

        document.ConfirmSecureStorageAndAudit(version.Id, stored.StorageReference, audit.Id);
        if (request.DocumentId is null)
            await documents.AddAsync(document, cancellationToken);
        else
            await documents.SaveChangesAsync(cancellationToken);

        return version;
    }
}

public sealed record AssociatedDocumentSubmission(
    Guid? DocumentId,
    string DossierId,
    string DocumentType,
    string AuthorId,
    DateTimeOffset SubmittedAt,
    string Channel,
    string OriginalFileName,
    string ContentType,
    long Length,
    string ContentHash,
    Stream Content,
    string? Reason = null);
