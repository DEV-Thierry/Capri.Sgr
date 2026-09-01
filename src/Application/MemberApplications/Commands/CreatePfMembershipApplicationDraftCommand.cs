using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.MemberApplications.Commands;

public sealed record CreatePfMembershipApplicationDraftCommand(string TenantId, string? ApplicantId, string Channel) : IRequest<MembershipApplication>;

public sealed class CreatePfMembershipApplicationDraftCommandHandler(PfMembershipApplicationService service) : IRequestHandler<CreatePfMembershipApplicationDraftCommand, MembershipApplication>
{
    public Task<MembershipApplication> Handle(CreatePfMembershipApplicationDraftCommand request, CancellationToken cancellationToken) =>
        service.CreateDraftAsync(request.TenantId, request.ApplicantId, request.Channel, cancellationToken);
}
