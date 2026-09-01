using Capri.Sgr.Application.AssociateMemberships;
using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Application.UnitTests.AssociateMemberships;

public class AssociateMembershipServiceTests
{
    [Test]
    public async Task PrefixRemainsUnchangedUntilInternalApprovalAndApprovalIsAudited()
    {
        var membership = NewMembership(AssociateType.Young);
        var proposals = new List<PrefixProposal>();
        var store = StoreFor(membership, proposals, []);
        store.Setup(x => x.PrefixIsInUseAsync("VALE", membership.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var audit = new Mock<IAuditStore>();
        var service = Service(store.Object, audit.Object);

        var proposalId = await service.ProposePrefixAsync(membership.Id, PrefixKind.Prefix, "Vale", CancellationToken.None);
        membership.PrefixDisplayValue.ShouldBeNull();
        store.Setup(x => x.FindPrefixProposalAsync(proposalId, It.IsAny<CancellationToken>())).ReturnsAsync(proposals.Single());

        await service.ApprovePrefixAsync(proposalId, CancellationToken.None);

        membership.PrefixDisplayValue.ShouldBe("Vale");
        audit.Verify(x => x.AppendAsync(It.Is<AuditRecord>(a => a.Action == "Proposta de Afixo aprovada"), CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task ReclassificationEvaluationIsIdempotentAndNeverChangesCurrentType()
    {
        var membership = NewMembership(AssociateType.ContributingJunior);
        var proposals = new List<ReclassificationProposal>();
        var store = StoreFor(membership, [], proposals);
        var service = Service(store.Object, new Mock<IAuditStore>().Object);

        await service.EvaluateReclassificationAsync(membership.Id, 60, CancellationToken.None);
        await service.EvaluateReclassificationAsync(membership.Id, 60, CancellationToken.None);

        proposals.Count.ShouldBe(1);
        proposals.Single().ProposedType.ShouldBe(AssociateType.ContributingSenior);
        membership.Type.ShouldBe(AssociateType.ContributingJunior);
    }

    [Test]
    public async Task ReclassificationCancelsOpenProposalWhenCountReturnsToCurrentRange()
    {
        var membership = NewMembership(AssociateType.ContributingJunior);
        var proposal = new ReclassificationProposal(membership.Id, AssociateType.ContributingSenior, 60, DateTimeOffset.UtcNow);
        var store = StoreFor(membership, [], [proposal]);
        var service = Service(store.Object, new Mock<IAuditStore>().Object);

        await service.EvaluateReclassificationAsync(membership.Id, 10, CancellationToken.None);

        proposal.Status.ShouldBe(ReclassificationProposalStatus.Cancelled);
    }

    private static AssociateMembership NewMembership(AssociateType type) => new("associate-1", AssociatePersonKind.NaturalPerson, type, new DateOnly(2026, 1, 1));
    private static AssociateMembershipService Service(IAssociateMembershipStore store, IAuditStore audit)
    {
        var clock = new Mock<IOperationalClock>(); clock.Setup(x => x.GetCurrentInstant()).Returns(new DateTimeOffset(2026, 3, 1, 9, 0, 0, TimeSpan.Zero)); clock.Setup(x => x.GetOperationalDate()).Returns(new DateOnly(2026, 3, 1));
        var user = new Mock<IUser>(); user.Setup(x => x.Id).Returns("operator-1"); return new AssociateMembershipService(store, audit, clock.Object, user.Object);
    }
    private static Mock<IAssociateMembershipStore> StoreFor(AssociateMembership membership, List<PrefixProposal> prefixProposals, List<ReclassificationProposal> reclassifications)
    {
        var store = new Mock<IAssociateMembershipStore>();
        store.Setup(x => x.FindAsync(membership.Id, It.IsAny<CancellationToken>())).ReturnsAsync(membership);
        store.Setup(x => x.AddPrefixProposalAsync(It.IsAny<PrefixProposal>(), It.IsAny<CancellationToken>())).Callback<PrefixProposal, CancellationToken>((p, _) => prefixProposals.Add(p)).Returns(Task.CompletedTask);
        store.Setup(x => x.AddReclassificationProposalAsync(It.IsAny<ReclassificationProposal>(), It.IsAny<CancellationToken>())).Callback<ReclassificationProposal, CancellationToken>((p, _) => reclassifications.Add(p)).Returns(Task.CompletedTask);
        store.Setup(x => x.FindOpenReclassificationAsync(membership.Id, It.IsAny<CancellationToken>())).ReturnsAsync(() => reclassifications.SingleOrDefault(x => x.Status == ReclassificationProposalStatus.Open));
        return store;
    }
}