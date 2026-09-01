using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.Common.Interfaces;

public interface IMembershipApplicationStore
{
    Task<MembershipApplication?> FindAsync(Guid applicationId, CancellationToken cancellationToken);
    Task<bool> CpfExistsAsync(string tenantId, string normalizedCpf, CancellationToken cancellationToken);
    Task AddAsync(MembershipApplication application, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
