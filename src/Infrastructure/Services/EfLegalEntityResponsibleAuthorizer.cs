using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Capri.Sgr.Infrastructure.Services;

public sealed class EfLegalEntityResponsibleAuthorizer(ApplicationDbContext context) : ILegalEntityResponsibleAuthorizer
{
    public async Task DemandActiveResponsibleAsync(string authenticatedUserId, Guid legalEntityApplicationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(authenticatedUserId);
        var permitted = await context.ResponsibleUserLinks.AnyAsync(link =>
            link.UserId == authenticatedUserId && link.ResponsiblePerson.LegalEntityApplications.Any(application =>
                application.LegalEntityMembershipApplicationId == legalEntityApplicationId && application.IsActive), cancellationToken);
        if (!permitted)
            throw new UnauthorizedAccessException("The authenticated user is not an active Responsible for this PJ application.");
    }
}