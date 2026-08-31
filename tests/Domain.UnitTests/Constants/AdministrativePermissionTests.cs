using Capri.Sgr.Domain.Constants;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Domain.UnitTests.Constants;

public class AdministrativePermissionTests
{
    [Test]
    public void CatalogueContainsEachIndependentActionForAdministrativeResources()
    {
        AdministrativePermission.All.ShouldBe([
            "internal-users:consult", "internal-users:maintain", "internal-users:decide", "internal-users:configure",
            "institutional-publications:consult", "institutional-publications:maintain", "institutional-publications:decide", "institutional-publications:configure"
        ], ignoreOrder: true);
    }

    [Test]
    public void PolicyHasStableAdministrativePrefix()
    {
        AdministrativePermission.Policy(AdministrativeResources.InternalUsers, AdministrativeActions.Configure)
            .ShouldBe("administrative:internal-users:configure");
    }
}
