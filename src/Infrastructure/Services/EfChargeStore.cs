using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Capri.Sgr.Infrastructure.Services;

public sealed class EfChargeStore(ApplicationDbContext context) : IChargeStore
{
    public Task<Charge?> FindAsync(Guid chargeId, CancellationToken cancellationToken) =>
        context.Charges.SingleOrDefaultAsync(charge => charge.Id == chargeId, cancellationToken);

    public Task AddAsync(Charge charge, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(charge);
        context.Charges.Add(charge);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await context.SaveChangesAsync(cancellationToken);
}
