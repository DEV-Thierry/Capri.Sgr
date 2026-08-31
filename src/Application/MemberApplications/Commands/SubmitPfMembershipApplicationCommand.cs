using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.MemberApplications.Commands;

public sealed record SubmitPfMembershipApplicationCommand(PfMembershipApplicationSubmission Submission) : IRequest<MembershipApplication>;

public sealed class SubmitPfMembershipApplicationCommandHandler(PfMembershipApplicationService service) : IRequestHandler<SubmitPfMembershipApplicationCommand, MembershipApplication>
{
    public Task<MembershipApplication> Handle(SubmitPfMembershipApplicationCommand request, CancellationToken cancellationToken) =>
        service.SubmitAsync(request.Submission, cancellationToken);
}

public sealed class SubmitPfMembershipApplicationCommandValidator : AbstractValidator<SubmitPfMembershipApplicationCommand>
{
    public SubmitPfMembershipApplicationCommandValidator()
    {
        RuleFor(command => command.Submission.MembershipType).NotEmpty();
        RuleFor(command => command.Submission.FullName).NotEmpty();
        RuleFor(command => command.Submission.Email).NotEmpty().EmailAddress();
        RuleFor(command => command.Submission.PrimaryPhone).NotEmpty();
        RuleFor(command => command.Submission.CorrespondenceAddress).NotEmpty();
        RuleFor(command => command.Submission.IdentityDocument).NotEmpty();
        RuleFor(command => command.Submission.TermVersion).NotEmpty();
        RuleFor(command => command.Submission.AcceptanceActorId).NotEmpty();
        RuleFor(command => command.Submission.Channel).NotEmpty();
    }
}
