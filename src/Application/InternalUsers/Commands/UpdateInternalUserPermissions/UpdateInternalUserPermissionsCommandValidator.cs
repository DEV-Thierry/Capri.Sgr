namespace Capri.Sgr.Application.InternalUsers.Commands.UpdateInternalUserPermissions;

public sealed class UpdateInternalUserPermissionsCommandValidator : AbstractValidator<UpdateInternalUserPermissionsCommand>
{
    public UpdateInternalUserPermissionsCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleForEach(command => command.Permissions).NotEmpty();
    }
}
