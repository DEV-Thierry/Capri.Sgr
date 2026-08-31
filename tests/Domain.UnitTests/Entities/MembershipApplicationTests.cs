using Capri.Sgr.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Domain.UnitTests.Entities;

public class MembershipApplicationTests
{
    [Test]
    public void SubmitRequiresCompletedPfDataAndExplicitTermAcceptance()
    {
        var application = new MembershipApplication("tenant-1", "applicant-1", Now);

        Should.Throw<InvalidOperationException>(() => application.Submit(Now, null));
        PopulateIdentity(application);
        Should.Throw<InvalidOperationException>(() => application.Submit(Now, null));

        application.AcceptTerm("2026-08", "applicant-1", "self-service", Now);
        application.Submit(Now, null);

        application.Status.ShouldBe(MembershipApplicationStatus.AwaitingDocuments);
        application.HasPermission(ProvisionalMembershipPermission.FollowUp).ShouldBeTrue();
        application.HasPermission(ProvisionalMembershipPermission.OperationalRequests).ShouldBeFalse();
        application.HasPermission(ProvisionalMembershipPermission.AnimalManagement).ShouldBeFalse();
    }

    [Test]
    public void ChargeApplicableSubmissionAwaitsPayment()
    {
        var application = new MembershipApplication("tenant-1", "applicant-1", Now);
        PopulateIdentity(application);
        application.AcceptTerm("2026-08", "applicant-1", "self-service", Now);
        var chargeId = Guid.NewGuid();

        application.Submit(Now, chargeId);

        application.Status.ShouldBe(MembershipApplicationStatus.AwaitingPayment);
        application.InitialChargeId.ShouldBe(chargeId);
        application.HasPermission(ProvisionalMembershipPermission.PayCharges).ShouldBeTrue();
    }

    private static void PopulateIdentity(MembershipApplication application) => application.UpdateIdentityAndContact(
        "529.982.247-25", "Maria da Silva", new DateOnly(1990, 1, 2), "maria@example.com", "+55 11 99999-9999", "Rua das Flores, 123", "RG 12.345.678-9");

    private static readonly DateTimeOffset Now = new(2026, 8, 30, 12, 0, 0, TimeSpan.Zero);
}
