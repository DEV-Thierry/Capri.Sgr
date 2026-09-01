using Capri.Sgr.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Domain.UnitTests.Entities;

public class AssociateMembershipTests
{
    [Test]
    public void ClosedCatalogueDefinesEligibilityChargesLimitsAndPrefixRules()
    {
        var junior = AssociateTypeCatalogue.Get(AssociateType.ContributingJunior);
        var life = AssociateTypeCatalogue.Get(AssociateType.LifeMember);

        junior.MinimumActiveHerd.ShouldBe(6);
        junior.MaximumActiveHerd.ShouldBe(59);
        junior.ChargeKind.ShouldBe(MembershipChargeKind.Quarterly);
        junior.ChargeAmount.ShouldBe(396m);
        junior.DiscountRate.ShouldBe(.50m);
        life.ChargeKind.ShouldBe(MembershipChargeKind.OneTime);
        life.ChargeAmount.ShouldBeNull();
        AssociateTypeCatalogue.IsEligible(AssociateType.Young, AssociatePersonKind.LegalEntity).ShouldBeFalse();
        AssociateTypeCatalogue.Get(AssociateType.User).AllowsPrefix.ShouldBeFalse();
    }

    [Test]
    public void PrefixNormalizesCaseAndAccentsButPreservesDisplayValue()
    {
        PrefixNormalizer.Normalize("  Águas do Sul  ").ShouldBe("AGUAS DO SUL");
        var membership = new AssociateMembership("associate-1", AssociatePersonKind.NaturalPerson, AssociateType.Young, new DateOnly(2026, 1, 1));
        var proposal = new PrefixProposal(membership.Id, PrefixKind.Prefix, "Águas do Sul", DateTimeOffset.UtcNow);
        proposal.Approve();

        membership.ApplyApprovedPrefix(proposal);

        membership.PrefixDisplayValue.ShouldBe("Águas do Sul");
        membership.PrefixNormalizedValue.ShouldBe("AGUAS DO SUL");
    }

    [Test]
    public void PrefixCannotChangeWithoutApprovedProposal()
    {
        var membership = new AssociateMembership("associate-1", AssociatePersonKind.NaturalPerson, AssociateType.Young, new DateOnly(2026, 1, 1));
        var proposal = new PrefixProposal(membership.Id, PrefixKind.Suffix, "Vale", DateTimeOffset.UtcNow);

        Should.Throw<InvalidOperationException>(() => membership.ApplyApprovedPrefix(proposal));
    }
}