using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Capri.Sgr.Infrastructure.Services;

public sealed class EfDerivedEffectExecutionStore(ApplicationDbContext context, IOperationalClock clock) : IDerivedEffectExecutionStore
{
    public async Task<DerivedEffectClaim> ClaimAsync(string executionKey, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executionKey);

        var execution = new DerivedEffectExecution(executionKey, clock.GetCurrentInstant());
        context.DerivedEffectExecutions.Add(execution);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return new DerivedEffectClaim(execution, true);
        }
        catch (DbUpdateException)
        {
            context.Entry(execution).State = EntityState.Detached;
            var existing = await context.DerivedEffectExecutions
                .SingleAsync(item => item.ExecutionKey == executionKey, cancellationToken);

            if (existing.Status == DerivedEffectExecutionStatus.Failed)
            {
                existing.Retry(clock.GetCurrentInstant());
                await context.SaveChangesAsync(cancellationToken);
                return new DerivedEffectClaim(existing, true);
            }

            return new DerivedEffectClaim(existing, false);
        }
    }

    public async Task MarkSucceededAsync(DerivedEffectExecution execution, CancellationToken cancellationToken)
    {
        execution.MarkSucceeded(clock.GetCurrentInstant());
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(DerivedEffectExecution execution, string failure, CancellationToken cancellationToken)
    {
        execution.MarkFailed(clock.GetCurrentInstant(), failure);
        await context.SaveChangesAsync(cancellationToken);
    }
}
