using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Application.MemberApplications;
using Capri.Sgr.Domain.Entities;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Application.UnitTests.MemberApplications;

public class PfMembershipApplicationServiceTests
{
    [Test]
    public async Task SubmissionCreatesInitialChargeAndProvisionalFollowUpEffects()
    {
        var application = new MembershipApplication("tenant-1", "applicant-1", Now);
        var applications = new Mock<IMembershipApplicationStore>();
        applications.Setup(store => store.FindAsync(application.Id, It.IsAny<CancellationToken>())).ReturnsAsync(application);
        applications.Setup(store => store.CpfExistsAsync("tenant-1", "52998224725", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var documents = new Mock<IAssociatedDocumentStore>();
        documents.Setup(store => store.HasCurrentDocumentAsync(application.DossierId, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var charges = new Mock<IChargeStore>();
        var policy = new Mock<IInitialMembershipChargePolicy>();
        policy.Setup(value => value.GetFor("Usuario", Now)).Returns(new InitialMembershipCharge(209m, Now.AddDays(10)));
        var pending = new Mock<IPendingItemStore>();
        var notifications = new Mock<INotificationStore>();
        var audit = new Mock<IAuditStore>();
        var clock = new Mock<IOperationalClock>();
        clock.Setup(value => value.GetCurrentInstant()).Returns(Now);
        var user = new Mock<IUser>();
        user.Setup(value => value.Id).Returns("applicant-1");
        var service = new PfMembershipApplicationService(applications.Object, documents.Object, charges.Object, policy.Object, pending.Object, notifications.Object, audit.Object, clock.Object, user.Object);

        var result = await service.SubmitAsync(Submission(application.Id), CancellationToken.None);

        result.Status.ShouldBe(MembershipApplicationStatus.AwaitingPayment);
        result.InitialChargeId.ShouldNotBeNull();
        charges.Verify(store => store.AddAsync(It.Is<Charge>(charge => charge.Amount == 209m && charge.PayerId == application.Id.ToString()), CancellationToken.None), Times.Once);
        pending.Verify(store => store.AddAsync(It.Is<PendingItem>(item => item.Impact.Contains("bloqueadas")), CancellationToken.None), Times.Once);
        notifications.Verify(store => store.AddAsync(It.Is<Notification>(notification => notification.Recipient == "maria@example.com"), CancellationToken.None), Times.Once);
        audit.Verify(store => store.AppendAsync(It.Is<AuditRecord>(record => record.Action == "Enviada"), CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task SubmissionFailsWhenAnyRequiredDocumentIsMissing()
    {
        var application = new MembershipApplication("tenant-1", "applicant-1", Now);
        var applications = new Mock<IMembershipApplicationStore>();
        applications.Setup(store => store.FindAsync(application.Id, It.IsAny<CancellationToken>())).ReturnsAsync(application);
        applications.Setup(store => store.CpfExistsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var documents = new Mock<IAssociatedDocumentStore>();
        documents.Setup(store => store.HasCurrentDocumentAsync(application.DossierId, "Identity", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        documents.Setup(store => store.HasCurrentDocumentAsync(application.DossierId, It.Is<string>(x => x != "Identity"), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var service = new PfMembershipApplicationService(applications.Object, documents.Object, Mock.Of<IChargeStore>(), Mock.Of<IInitialMembershipChargePolicy>(), Mock.Of<IPendingItemStore>(), Mock.Of<INotificationStore>(), Mock.Of<IAuditStore>(), Clock().Object, User().Object);

        await Should.ThrowAsync<Capri.Sgr.Application.Common.Exceptions.BusinessRuleValidationException>(() => service.SubmitAsync(Submission(application.Id), CancellationToken.None));
    }

    private static PfMembershipApplicationSubmission Submission(Guid id) => new(id, "Usuario", "529.982.247-25", "Maria da Silva", new DateOnly(1990, 1, 2), "maria@example.com", "+55 11 99999-9999", "Rua das Flores, 123", "RG 12.345.678-9", "2026-08", "applicant-1", "self-service");
    private static Mock<IOperationalClock> Clock() { var clock = new Mock<IOperationalClock>(); clock.Setup(value => value.GetCurrentInstant()).Returns(Now); return clock; }
    private static Mock<IUser> User() { var user = new Mock<IUser>(); user.Setup(value => value.Id).Returns("applicant-1"); return user; }
    private static readonly DateTimeOffset Now = new(2026, 8, 30, 12, 0, 0, TimeSpan.Zero);
}
