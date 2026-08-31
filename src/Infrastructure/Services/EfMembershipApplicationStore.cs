using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Capri.Sgr.Infrastructure.Services;

public sealed class EfMembershipApplicationStore(ApplicationDbContext context) : IMembershipApplicationStore
{
    public Task<MembershipApplication?> FindAsync(Guid applicationId, CancellationToken cancellationToken) =>
        context.MembershipApplications.SingleOrDefaultAsync(application => application.Id == applicationId, cancellationToken);

    public Task<bool> CpfExistsAsync(string tenantId, string normalizedCpf, CancellationToken cancellationToken) =>
        context.MembershipApplications.AnyAsync(application => application.TenantId == tenantId && application.Cpf == normalizedCpf, cancellationToken);

    public Task AddAsync(MembershipApplication application, CancellationToken cancellationToken)
    {
        context.MembershipApplications.Add(application);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
