using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Data;

namespace Capri.Sgr.Infrastructure.Services;

public sealed class EfAuditStore(ApplicationDbContext context) : IAuditStore
{
    public async Task AppendAsync(AuditRecord record, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(record);
        context.AuditRecords.Add(record);
        await context.SaveChangesAsync(cancellationToken);
    }
}
