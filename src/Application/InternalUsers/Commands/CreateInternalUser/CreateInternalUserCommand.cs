using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Application.Common.Models;
using Capri.Sgr.Application.Common.Security;
using Capri.Sgr.Domain.Constants;

namespace Capri.Sgr.Application.InternalUsers.Commands.CreateInternalUser;

[Authorize(Policy = "administrative:internal-users:maintain")]
public sealed record CreateInternalUserCommand(string UserName, string Password, IReadOnlyCollection<string> Permissions) : IRequest<InternalUser>;

public sealed class CreateInternalUserCommandHandler(IIdentityService identityService) : IRequestHandler<CreateInternalUserCommand, InternalUser>
{
    public async Task<InternalUser> Handle(CreateInternalUserCommand request, CancellationToken cancellationToken)
    {
        var (result, userId) = await identityService.CreateInternalUserAsync(request.UserName, request.Password, request.Permissions, cancellationToken);
        if (!result.Succeeded) throw new ValidationException([new FluentValidation.Results.ValidationFailure("InternalUser", string.Join(" ", result.Errors))]);

        return new InternalUser(userId, request.UserName, true, request.Permissions);
    }
}