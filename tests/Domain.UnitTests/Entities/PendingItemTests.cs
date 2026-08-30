using Capri.Sgr.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Domain.UnitTests.Entities;

public class PendingItemTests
{
    [Test]
    public void OpenPendingItemExplainsCauseImpactAndRegularizationUntilResolved()
    {
        var openedAt = new DateTimeOffset(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
        var pendingItem = new PendingItem(
            "Cobrança inicial vencida",
            "A aprovação da solicitação permanece bloqueada",
            "Registre a quitação da cobrança inicial",
            openedAt);

        pendingItem.Status.ShouldBe(PendingItemStatus.Open);
        pendingItem.Cause.ShouldBe("Cobrança inicial vencida");
        pendingItem.Impact.ShouldBe("A aprovação da solicitação permanece bloqueada");
        pendingItem.Regularization.ShouldBe("Registre a quitação da cobrança inicial");

        pendingItem.Resolve(openedAt.AddDays(1), "Pagamento confirmado");

        pendingItem.Status.ShouldBe(PendingItemStatus.Resolved);
        pendingItem.ClosureReason.ShouldBe("Pagamento confirmado");
        pendingItem.ClosedAt.ShouldBe(openedAt.AddDays(1));
    }

    [Test]
    public void ClosedPendingItemCannotBeClosedAgain()
    {
        var pendingItem = new PendingItem("Documento inválido", "Envio bloqueado", "Reenvie o documento", DateTimeOffset.UtcNow);
        pendingItem.Cancel(DateTimeOffset.UtcNow, "Solicitação cancelada");

        Should.Throw<InvalidOperationException>(() => pendingItem.Resolve(DateTimeOffset.UtcNow, "Documento aprovado"));
    }
}
