using Capri.Sgr.Domain.ValueObjects;

namespace Capri.Sgr.Domain.Entities;

/// <summary>Reusable natural-person identity that can act for one or more legal entities.</summary>
public sealed class ResponsiblePerson
{
    private ResponsiblePerson() { }

    public ResponsiblePerson(Cpf cpf, string fullName, string email, string phone)
    {
        ArgumentNullException.ThrowIfNull(cpf);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);
        Id = Guid.NewGuid();
        Cpf = cpf.Value;
        FullName = fullName.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
    }

    private readonly List<LegalEntityResponsible> _legalEntityApplications = [];

    public Guid Id { get; private set; }
    public string Cpf { get; private set; } = null!;
    public IReadOnlyCollection<LegalEntityResponsible> LegalEntityApplications => _legalEntityApplications.AsReadOnly();
    public string FullName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
}