using Capri.Sgr.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Domain.UnitTests.Entities;

public class ChargeTests
{
    private static readonly DateTimeOffset IssuedAt = new(2026, 4, 1, 9, 0, 0, TimeSpan.FromHours(-3));
    private static readonly DateTimeOffset DueAt = new(2026, 4, 16, 23, 59, 59, TimeSpan.FromHours(-3));

    [Test]
    public void CreatePreservesPayerDueDateAndAllGeneratorFacts()
    {
        var facts = new[]
        {
            new ChargeGeneratorFact("Cobertura", "coverage-42", "PreRegistro"),
            new ChargeGeneratorFact("Cobertura", "coverage-42", "MultaComunicacaoTardia")
        };

        var charge = Charge.Create("associate-7", 250.00m, DueAt, facts, IssuedAt);

        charge.PayerId.ShouldBe("associate-7");
        charge.Amount.ShouldBe(250.00m);
        charge.DueAt.ShouldBe(DueAt);
        charge.Status.ShouldBe(ChargeStatus.Open);
        charge.GeneratorFacts.ShouldBe(facts);
    }

    [Test]
    public void RecordPaymentPaysChargeOnceAndPreservesEvidence()
    {
        var charge = CreateCharge();
        var paidAt = IssuedAt.AddDays(2);

        charge.RecordPayment("receipt-2026-0001", "Comprovante bancário #42", paidAt);

        charge.Status.ShouldBe(ChargeStatus.Paid);
        charge.Payment.ShouldNotBeNull();
        charge.Payment!.Reference.ShouldBe("receipt-2026-0001");
        charge.Payment.Evidence.ShouldBe("Comprovante bancário #42");
        charge.Payment.RecordedAt.ShouldBe(paidAt);
    }

    [Test]
    public void RecordPaymentTwiceIsRejected()
    {
        var charge = CreateCharge();
        charge.RecordPayment("receipt-2026-0001", "Comprovante bancário #42", IssuedAt);

        Should.Throw<InvalidOperationException>(() =>
            charge.RecordPayment("receipt-2026-0002", "Outro comprovante", IssuedAt.AddMinutes(1)));
    }

    [Test]
    public void CancellationRequiresReasonAndEvidenceAndPreservesDecision()
    {
        var charge = CreateCharge();

        charge.Cancel("Cobrança emitida por engano", "Solicitação administrativa #18", IssuedAt.AddDays(1));

        charge.Status.ShouldBe(ChargeStatus.Cancelled);
        charge.Cancellation.ShouldNotBeNull();
        charge.Cancellation!.Reason.ShouldBe("Cobrança emitida por engano");
        charge.Cancellation.Evidence.ShouldBe("Solicitação administrativa #18");
    }

    [Test]
    public void PaidChargeCanBeRefundedWithReasonAndEvidence()
    {
        var charge = CreateCharge();
        charge.RecordPayment("receipt-2026-0001", "Comprovante bancário #42", IssuedAt);

        charge.Refund("Pagamento indevido", "Comprovante de estorno #99", IssuedAt.AddDays(3));

        charge.Status.ShouldBe(ChargeStatus.Refunded);
        charge.RefundDecision.ShouldNotBeNull();
        charge.RefundDecision!.Reason.ShouldBe("Pagamento indevido");
        charge.RefundDecision.Evidence.ShouldBe("Comprovante de estorno #99");
    }

    [Test]
    public void DueOpenChargeProducesPendingAndNotificationSeamWithoutChangingLifecycle()
    {
        var charge = CreateCharge();

        var effect = charge.GetDueEffect(DueAt.AddSeconds(1));

        effect.ShouldNotBeNull();
        effect!.Pending.ShouldBe(new FinancialPending(charge.Id, "Vencimento", "Cobrança vencida"));
        effect.Notification.ShouldBe(new ChargeDueNotification(charge.Id, "associate-7", DueAt));
        charge.Status.ShouldBe(ChargeStatus.Open);
    }

    [Test]
    public void DueEffectIsNotProducedForPaidOrNotYetDueCharge()
    {
        var charge = CreateCharge();

        charge.GetDueEffect(DueAt).ShouldBeNull();
        charge.RecordPayment("receipt-2026-0001", "Comprovante bancário #42", IssuedAt);
        charge.GetDueEffect(DueAt.AddDays(1)).ShouldBeNull();
    }

    private static Charge CreateCharge() => Charge.Create(
        "associate-7",
        250.00m,
        DueAt,
        [new ChargeGeneratorFact("Cobertura", "coverage-42", "PreRegistro")],
        IssuedAt);
}
