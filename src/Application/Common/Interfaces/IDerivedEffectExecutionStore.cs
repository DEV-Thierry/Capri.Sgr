using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.Common.Interfaces;

public sealed record DerivedEffectClaim(DerivedEffectExecution Execution, bool ShouldProcess);

/// <summary>Claims idempotent effects and makes failed effects reprocessable.</summary>
public interface IDerivedEffectExecutionStore
{
    Task<DerivedEffectClaim> ClaimAsync(string executionKey, CancellationToken cancellationToken);
    Task MarkSucceededAsync(DerivedEffectExecution execution, CancellationToken cancellationToken);
    Task MarkFailedAsync(DerivedEffectExecution execution, string failure, CancellationToken cancellationToken);
}
