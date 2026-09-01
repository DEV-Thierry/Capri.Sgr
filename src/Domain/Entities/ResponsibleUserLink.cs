namespace Capri.Sgr.Domain.Entities;

/// <summary>Authenticated access is explicitly linked to a reusable Responsible identity.</summary>
public sealed class ResponsibleUserLink
{
    private ResponsibleUserLink() { }
    public ResponsibleUserLink(Guid responsiblePersonId, string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ResponsiblePersonId = responsiblePersonId;
        UserId = userId;
    }
    public Guid ResponsiblePersonId { get; private set; }
    public ResponsiblePerson ResponsiblePerson { get; private set; } = null!;
    public string UserId { get; private set; } = null!;
}