namespace Capri.Sgr.Application.InternalUsers.Commands.CreateInternalUser;

public sealed class CreateInternalUserCommandValidator : AbstractValidator<CreateInternalUserCommand>
{
    public CreateInternalUserCommandValidator()
    {
        RuleFor(command => command.UserName).NotEmpty().EmailAddress();
        RuleFor(command => command.Password).NotEmpty().MinimumLength(8);
        RuleForEach(command => command.Permissions).NotEmpty();
    }
}
