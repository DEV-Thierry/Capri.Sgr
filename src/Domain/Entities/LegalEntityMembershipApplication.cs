namespace Capri.Sgr.Domain.Entities;

public enum LegalEntityApplicationStatus { Draft, Submitted, Approved }

/// <summary>Focused PJ application aggregate; it deliberately does not model PF applications.</summary>
public sealed class LegalEntityMembershipApplication
{
    public static readonly IReadOnlySet<string> RequiredDocumentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "CNPJ", "ContratoOuEstatuto", "ComprovanteDeEndereco", "TermoDeAdesao", "IdentidadeResponsavelPrincipal"
    };

    private readonly List<LegalEntityResponsible> _responsibles = [];
    private LegalEntityMembershipApplication() { }

    public LegalEntityMembershipApplication(string tenantId, string cnpj, string corporateName, string institutionalEmail, string dossierId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(corporateName);
        ArgumentException.ThrowIfNullOrWhiteSpace(institutionalEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(dossierId);
        Id = Guid.NewGuid();
        TenantId = tenantId;
        Cnpj = new ValueObjects.Cnpj(cnpj).Value;
        CorporateName = corporateName.Trim();
        InstitutionalEmail = institutionalEmail.Trim();
        DossierId = dossierId;
        Status = LegalEntityApplicationStatus.Draft;
    }

    public Guid Id { get; private set; }
    public string TenantId { get; private set; } = null!;
    public string Cnpj { get; private set; } = null!;
    public string CorporateName { get; private set; } = null!;
    public string InstitutionalEmail { get; private set; } = null!;
    public string DossierId { get; private set; } = null!;
    public LegalEntityApplicationStatus Status { get; private set; }
    public IReadOnlyCollection<LegalEntityResponsible> Responsibles => _responsibles.AsReadOnly();

    public void AddResponsible(ResponsiblePerson person, bool isPrincipal)
    {
        ArgumentNullException.ThrowIfNull(person);
        if (_responsibles.Any(link => link.ResponsiblePersonId == person.Id))
            throw new InvalidOperationException("The responsible person is already linked to this PJ application.");
        if (isPrincipal && _responsibles.Any(link => link.IsPrincipal && link.IsActive))
            throw new InvalidOperationException("A PJ application can have only one active principal responsible.");
        _responsibles.Add(new LegalEntityResponsible(person, isPrincipal));
    }

    public void DeactivateResponsible(Guid responsiblePersonId)
    {
        var link = FindResponsible(responsiblePersonId);
        link.Deactivate();
    }

    public void Submit(IReadOnlyCollection<string> securelyStoredDocumentTypes)
    {
        EnsureReadyForDecision(securelyStoredDocumentTypes, "submission");
        Status = LegalEntityApplicationStatus.Submitted;
    }

    public void Approve(IReadOnlyCollection<string> approvedDocumentTypes)
    {
        if (Status != LegalEntityApplicationStatus.Submitted)
            throw new InvalidOperationException("Only a submitted PJ application can be approved.");
        EnsureReadyForDecision(approvedDocumentTypes, "approval");
        Status = LegalEntityApplicationStatus.Approved;
    }

    public bool HasActiveResponsible(Guid responsiblePersonId) => _responsibles.Any(link => link.ResponsiblePersonId == responsiblePersonId && link.IsActive);

    private LegalEntityResponsible FindResponsible(Guid personId) => _responsibles.SingleOrDefault(link => link.ResponsiblePersonId == personId)
        ?? throw new KeyNotFoundException("The responsible person is not linked to this PJ application.");

    private void EnsureReadyForDecision(IReadOnlyCollection<string> documentTypes, string action)
    {
        ArgumentNullException.ThrowIfNull(documentTypes);
        if (_responsibles.Count(link => link.IsPrincipal && link.IsActive) != 1)
            throw new InvalidOperationException($"Exactly one active principal responsible is required for {action}.");
        var supplied = new HashSet<string>(documentTypes, StringComparer.OrdinalIgnoreCase);
        var missing = RequiredDocumentTypes.Where(type => !supplied.Contains(type)).ToArray();
        if (missing.Length != 0)
            throw new InvalidOperationException($"PJ application cannot proceed with {action}; missing required documents: {string.Join(", ", missing)}.");
    }
}

public sealed class LegalEntityResponsible
{
    private LegalEntityResponsible() { }
    internal LegalEntityResponsible(ResponsiblePerson person, bool isPrincipal)
    {
        ResponsiblePerson = person;
        ResponsiblePersonId = person.Id;
        IsPrincipal = isPrincipal;
        IsActive = true;
    }
    public Guid LegalEntityMembershipApplicationId { get; private set; }
    public Guid ResponsiblePersonId { get; private set; }
    public ResponsiblePerson ResponsiblePerson { get; private set; } = null!;
    public bool IsPrincipal { get; private set; }
    public bool IsActive { get; private set; }
    internal void Deactivate() => IsActive = false;
}