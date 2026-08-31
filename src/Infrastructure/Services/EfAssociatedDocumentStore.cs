using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Capri.Sgr.Infrastructure.Services;

public sealed class EfAssociatedDocumentStore(ApplicationDbContext context) : IAssociatedDocumentStore
{
    public Task<AssociatedDocument?> FindAsync(Guid documentId, CancellationToken cancellationToken) =>
        context.AssociatedDocuments
            .Include(document => document.Versions)
            .SingleOrDefaultAsync(document => document.Id == documentId, cancellationToken);

    public async Task<IReadOnlyCollection<AssociatedDocument>> ListByDossierAsync(string dossierId, CancellationToken cancellationToken) =>
        await context.AssociatedDocuments
            .Include(document => document.Versions)
            .Where(document => document.DossierId == dossierId)
            .ToListAsync(cancellationToken);

    public Task AddAsync(AssociatedDocument document, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);
        context.AssociatedDocuments.Add(document);
        return context.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await context.SaveChangesAsync(cancellationToken);
}
