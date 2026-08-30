using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.Common.Interfaces;

/// <summary>Append-only seam for recording Auditoria.</summary>
public interface IAuditStore
{
    Task AppendAsync(AuditRecord record, CancellationToken cancellationToken);
}
