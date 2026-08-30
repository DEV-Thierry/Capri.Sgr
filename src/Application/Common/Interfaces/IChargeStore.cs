using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.Common.Interfaces;

/// <summary>Persistence seam for Cobranças; implementations enforce generator-fact idempotency.</summary>
public interface IChargeStore
{
    Task<Charge?> FindAsync(Guid chargeId, CancellationToken cancellationToken);
    Task AddAsync(Charge charge, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
