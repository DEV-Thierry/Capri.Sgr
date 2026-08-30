using Capri.Sgr.Application.Common.Exceptions;
using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Application.InternalUsers.Commands.UpdateInternalUserPermissions;
using Capri.Sgr.Domain.Constants;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Application.UnitTests.InternalUsers;

public class UpdateInternalUserPermissionsCommandHandlerTests
{
    [Test]
    public async Task DelegatesPermissionReplacementUsingCurrentUserAsActor()
    {
        var identityService = new Mock<IIdentityService>();
        identityService.Setup(service => service.UpdateInternalUserPermissionsAsync("actor", "target", It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Capri.Sgr.Application.Common.Models.Result.Success());
        var currentUser = new Mock<IUser>();
        currentUser.SetupGet(user => user.Id).Returns("actor");
        var handler = new UpdateInternalUserPermissionsCommandHandler(identityService.Object, currentUser.Object);
        var permissions = new[] { AdministrativePermission.Create(AdministrativeResources.InternalUsers, AdministrativeActions.Consult) };

        await handler.Handle(new UpdateInternalUserPermissionsCommand("target", permissions), CancellationToken.None);

        identityService.Verify(service => service.UpdateInternalUserPermissionsAsync("actor", "target", permissions, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void RejectsAnonymousActor()
    {
        var identityService = new Mock<IIdentityService>();
        var currentUser = new Mock<IUser>();
        var handler = new UpdateInternalUserPermissionsCommandHandler(identityService.Object, currentUser.Object);

        Should.ThrowAsync<UnauthorizedAccessException>(() => handler.Handle(new UpdateInternalUserPermissionsCommand("target", []), CancellationToken.None));
    }
}
