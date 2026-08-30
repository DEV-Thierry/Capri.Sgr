using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.Charges;

/// <summary>Coordinates manual financial decisions and emits immutable Auditoria evidence.</summary>
public sealed class ManualChargeService(IChargeStore charges, IAuditStore audit, IOperationalClock clock, IUser user)
{
    public async Task RecordPaymentAsync(Guid chargeId, string reference, string evidence, CancellationToken cancellationToken)
    {
        var charge = await GetChargeAsync(chargeId, cancellationToken);
        var before = charge.Status.ToString();
        charge.RecordPayment(reference, evidence, clock.GetCurrentInstant());
        await charges.SaveChangesAsync(cancellationToken);
        await AppendAuditAsync(charge, "Pagamento registrado", evidence, before, cancellationToken);
    }

    public async Task CancelAsync(Guid chargeId, string reason, string evidence, CancellationToken cancellationToken)
    {
        var charge = await GetChargeAsync(chargeId, cancellationToken);
        var before = charge.Status.ToString();
        charge.Cancel(reason, evidence, clock.GetCurrentInstant());
        await charges.SaveChangesAsync(cancellationToken);
        await AppendAuditAsync(charge, "Cobrança cancelada", reason, before, cancellationToken);
    }

    public async Task RefundAsync(Guid chargeId, string reason, string evidence, CancellationToken cancellationToken)
    {
        var charge = await GetChargeAsync(chargeId, cancellationToken);
        var before = charge.Status.ToString();
        charge.Refund(reason, evidence, clock.GetCurrentInstant());
        await charges.SaveChangesAsync(cancellationToken);
        await AppendAuditAsync(charge, "Cobrança estornada", reason, before, cancellationToken);
    }

    private async Task<Charge> GetChargeAsync(Guid chargeId, CancellationToken cancellationToken) =>
        await charges.FindAsync(chargeId, cancellationToken)
        ?? throw new KeyNotFoundException($"Cobrança '{chargeId}' was not found.");

    private Task AppendAuditAsync(Charge charge, string action, string reason, string before, CancellationToken cancellationToken) =>
        audit.AppendAsync(
            new AuditRecord(user.Id, clock.GetCurrentInstant(), "manual", "Cobrança", charge.Id.ToString(), action, reason, $"{{\"situacao\":\"{before}\"}}", $"{{\"situacao\":\"{charge.Status}\"}}"),
            cancellationToken);
}
