using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.LegalEntityApplications;

/// <summary>Coordinates PJ status changes with the versioned-document capability and Auditoria.</summary>
public sealed class LegalEntityApplicationService(
    IApplicationDbContext context,
    IAssociatedDocumentStore documents,
    IAuditStore audit,
    IOperationalClock clock,
    IUser user,
    ILegalEntityResponsibleAuthorizer responsibleAuthorizer)
{
    public async Task SubmitAsync(Guid applicationId, CancellationToken cancellationToken)
    {
        var application = await FindAsync(applicationId, cancellationToken);
        await responsibleAuthorizer.DemandActiveResponsibleAsync(user.Id ?? throw new UnauthorizedAccessException(), applicationId, cancellationToken);
        var before = application.Status.ToString();
        application.Submit(await GetCurrentDocumentTypesAsync(application, false, cancellationToken));
        await context.SaveChangesAsync(cancellationToken);
        await AuditAsync(application, "Solicitação PJ enviada", before, cancellationToken);
    }

    public async Task ApproveAsync(Guid applicationId, CancellationToken cancellationToken)
    {
        var application = await FindAsync(applicationId, cancellationToken);
        var before = application.Status.ToString();
        application.Approve(await GetCurrentDocumentTypesAsync(application, true, cancellationToken));
        await context.SaveChangesAsync(cancellationToken);
        await AuditAsync(application, "Solicitação PJ aprovada", before, cancellationToken);
    }

    private async Task<IReadOnlyCollection<string>> GetCurrentDocumentTypesAsync(LegalEntityMembershipApplication application, bool approved, CancellationToken cancellationToken) =>
        (await documents.ListByDossierAsync(application.DossierId, cancellationToken))
            .Where(document => document.CurrentVersion is not null && (!approved || document.HasApprovedCurrentVersion()))
            .Select(document => document.DocumentType)
            .ToArray();

    private async Task<LegalEntityMembershipApplication> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await context.LegalEntityMembershipApplications.Include(application => application.Responsibles).SingleOrDefaultAsync(application => application.Id == id, cancellationToken)
        ?? throw new KeyNotFoundException($"PJ application '{id}' was not found.");

    private Task AuditAsync(LegalEntityMembershipApplication application, string action, string before, CancellationToken cancellationToken) =>
        audit.AppendAsync(new AuditRecord(user.Id, clock.GetCurrentInstant(), "authenticated", "SolicitaçãoDeAssociacaoPJ", application.Id.ToString(), action, after: "{\"situacaoAnterior\":\"" + before + "\",\"situacaoAtual\":\"" + application.Status + "\"}"), cancellationToken);
}