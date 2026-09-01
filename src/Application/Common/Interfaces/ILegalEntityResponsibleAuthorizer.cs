namespace Capri.Sgr.Application.Common.Interfaces;

/// <summary>Authorization seam for an authenticated Responsible acting for a PJ.</summary>
public interface ILegalEntityResponsibleAuthorizer
{
    Task DemandActiveResponsibleAsync(string authenticatedUserId, Guid legalEntityApplicationId, CancellationToken cancellationToken);
}