using Capri.Sgr.Application.Charges;
using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Application.UnitTests.Charges;

public class ManualChargeServiceTests
{
    [Test]
    public async Task PaymentIsRecordedOnceAndAppendsAuditEvidence()
    {
        var charge = CreateCharge();
        var charges = new Mock<IChargeStore>();
        charges.Setup(store => store.FindAsync(charge.Id, It.IsAny<CancellationToken>())).ReturnsAsync(charge);
        var audit = new Mock<IAuditStore>();
        var clock = new Mock<IOperationalClock>();
        clock.Setup(value => value.GetCurrentInstant()).Returns(new DateTimeOffset(2026, 4, 2, 9, 0, 0, TimeSpan.FromHours(-3)));
        var user = new Mock<IUser>();
        user.Setup(value => value.Id).Returns("operator-7");
        var service = new ManualChargeService(charges.Object, audit.Object, clock.Object, user.Object);

        await service.RecordPaymentAsync(charge.Id, "receipt-2026-0001", "Comprovante bancário #42", CancellationToken.None);

        charge.Status.ShouldBe(ChargeStatus.Paid);
        charges.Verify(store => store.SaveChangesAsync(CancellationToken.None), Times.Once);
        audit.Verify(store => store.AppendAsync(
            It.Is<AuditRecord>(record =>
                record.ActorId == "operator-7" &&
                record.EntityType == "Cobrança" &&
                record.Action == "Pagamento registrado" &&
                record.Reason == "Comprovante bancário #42"),
            CancellationToken.None), Times.Once);
    }

    private static Charge CreateCharge() => Charge.Create(
        "associate-7",
        250.00m,
        new DateTimeOffset(2026, 4, 16, 23, 59, 59, TimeSpan.FromHours(-3)),
        [new ChargeGeneratorFact("Cobertura", "coverage-42", "PreRegistro")],
        new DateTimeOffset(2026, 4, 1, 9, 0, 0, TimeSpan.FromHours(-3)));
}
