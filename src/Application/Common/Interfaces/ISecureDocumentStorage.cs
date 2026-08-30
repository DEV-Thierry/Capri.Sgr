namespace Capri.Sgr.Application.Common.Interfaces;

/// <summary>Stores a document only after provider-side security checks succeed.</summary>
public interface ISecureDocumentStorage
{
    Task<SecureDocumentStorageConfirmation> StoreAsync(
        SecureDocumentStorageRequest request,
        CancellationToken cancellationToken);
}

public sealed record SecureDocumentStorageRequest(
    string DossierId,
    string OriginalFileName,
    string ContentType,
    long Length,
    string ContentHash,
    Stream Content);

public sealed record SecureDocumentStorageConfirmation(string StorageReference);
