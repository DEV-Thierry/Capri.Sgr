using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.Common.Interfaces;

/// <summary>Persistence seam for preserved document dossiers.</summary>
public interface IAssociatedDocumentStore
{
    Task<AssociatedDocument?> FindAsync(Guid documentId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<AssociatedDocument>> ListByDossierAsync(string dossierId, CancellationToken cancellationToken);
    /// <summary>Checks the #5 document seam for a securely stored current version.</summary>
    Task<bool> HasCurrentDocumentAsync(string dossierId, string documentType, CancellationToken cancellationToken);
    Task AddAsync(AssociatedDocument document, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
