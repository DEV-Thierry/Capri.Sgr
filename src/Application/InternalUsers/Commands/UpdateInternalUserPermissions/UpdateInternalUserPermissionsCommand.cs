using Capri.Sgr.Application.Common.Exceptions;
using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Application.Common.Security;
using Capri.Sgr.Domain.Constants;

namespace Capri.Sgr.Application.InternalUsers.Commands.UpdateInternalUserPermissions;

[Authorize(Policy = "administrative:internal-users:configure")]
public sealed record UpdateInternalUserPermissionsCommand(string UserId, IReadOnlyCollection<string> Permissions) : IRequest;

public sealed class UpdateInternalUserPermissionsCommandHandler(IIdentityService identityService, IUser user) : IRequestHandler<UpdateInternalUserPermissionsCommand>
{
    public async Task Handle(UpdateInternalUserPermissionsCommand request, CancellationToken cancellationToken)
    {
        if (user.Id is null) throw new UnauthorizedAccessException();

        var result = await identityService.UpdateInternalUserPermissionsAsync(user.Id, request.UserId, request.Permissions, cancellationToken);
        if (!result.Succeeded) throw new BusinessRuleValidationException(string.Join(" ", result.Errors));
    }
}