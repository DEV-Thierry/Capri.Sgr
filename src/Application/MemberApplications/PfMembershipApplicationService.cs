using Capri.Sgr.Application.Common.Exceptions;
using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.MemberApplications;

public sealed class PfMembershipApplicationService(
    IMembershipApplicationStore applications,
    IAssociatedDocumentStore documents,
    IChargeStore charges,
    IInitialMembershipChargePolicy initialChargePolicy,
    IPendingItemStore pendingItems,
    INotificationStore notifications,
    IAuditStore audit,
    IOperationalClock clock,
    IUser user)
{
    private static readonly string[] RequiredDocumentTypes = ["Cpf", "Identity", "ProofOfResidence", "MembershipTerm"];

    public async Task<MembershipApplication> CreateDraftAsync(string tenantId, string? applicantId, string channel, CancellationToken cancellationToken)
    {
        var application = new MembershipApplication(tenantId, applicantId, clock.GetCurrentInstant());
        await applications.AddAsync(application, cancellationToken);
        await audit.AppendAsync(new AuditRecord(user.Id, clock.GetCurrentInstant(), channel, "Solicitação de associação", application.Id.ToString(), "Rascunho criado"), cancellationToken);
        return application;
    }

    public async Task<MembershipApplication> SubmitAsync(PfMembershipApplicationSubmission submission, CancellationToken cancellationToken)
    {
        var application = await applications.FindAsync(submission.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Solicitação '{submission.ApplicationId}' was not found.");

        application.UpdateIdentityAndContact(submission.Cpf, submission.FullName, submission.BirthDate, submission.Email,
            submission.PrimaryPhone, submission.CorrespondenceAddress, submission.IdentityDocument);
        if (await applications.CpfExistsAsync(application.TenantId, application.Cpf!, cancellationToken))
            throw new BusinessRuleValidationException("CPF already belongs to a membership application in this association.");

        application.AcceptTerm(submission.TermVersion, submission.AcceptanceActorId, submission.Channel, clock.GetCurrentInstant());
        foreach (var documentType in RequiredDocumentTypes)
            if (!await documents.HasCurrentDocumentAsync(application.DossierId, documentType, cancellationToken))
                throw new BusinessRuleValidationException($"Required document '{documentType}' is missing or not stored.");

        var submittedAt = clock.GetCurrentInstant();
        var terms = initialChargePolicy.GetFor(submission.MembershipType, submittedAt);
        Charge? charge = null;
        if (terms is not null)
        {
            charge = Charge.Create(application.Id.ToString(), terms.Amount, terms.DueAt,
                [new ChargeGeneratorFact("SolicitaçãoDeAssociacao", application.Id.ToString(), "CobrancaInicial")], submittedAt);
            await charges.AddAsync(charge, cancellationToken);
        }

        application.Submit(submittedAt, charge?.Id);
        await applications.SaveChangesAsync(cancellationToken);
        if (charge is not null)
            await charges.SaveChangesAsync(cancellationToken);

        await pendingItems.AddAsync(new PendingItem("Solicitação de associação enviada", "Operações dependentes de aprovação permanecem bloqueadas.", "Acompanhar documentos, pagamento e aprovação.", submittedAt), cancellationToken);
        await pendingItems.SaveChangesAsync(cancellationToken);
        await notifications.AddAsync(new Notification("MembershipApplicationSubmitted", application.Email!, "Email", "Sua solicitação de associação foi enviada.", submittedAt), cancellationToken);
        await notifications.SaveChangesAsync(cancellationToken);
        var after = "{\"status\":\"" + application.Status + "\",\"chargeId\":\"" + charge?.Id + "\"}";
        await audit.AppendAsync(new AuditRecord(user.Id, submittedAt, submission.Channel, "Solicitação de associação", application.Id.ToString(), "Enviada", after: after), cancellationToken);

        return application;
    }
}

public sealed record PfMembershipApplicationSubmission(
    Guid ApplicationId,
    string MembershipType,
    string Cpf,
    string FullName,
    DateOnly BirthDate,
    string Email,
    string PrimaryPhone,
    string CorrespondenceAddress,
    string IdentityDocument,
    string TermVersion,
    string AcceptanceActorId,
    string Channel);
