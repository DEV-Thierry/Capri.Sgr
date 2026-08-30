using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Application.Common.Security;
using Capri.Sgr.Domain.Constants;

namespace Capri.Sgr.Application.InternalUsers.Queries.GetInternalUsers;

[Authorize(Policy = "administrative:internal-users:consult")]
public sealed record GetInternalUsersQuery : IRequest<IReadOnlyCollection<InternalUser>>;

public sealed class GetInternalUsersQueryHandler(IIdentityService identityService) : IRequestHandler<GetInternalUsersQuery, IReadOnlyCollection<InternalUser>>
{
    public Task<IReadOnlyCollection<InternalUser>> Handle(GetInternalUsersQuery request, CancellationToken cancellationToken) =>
        identityService.GetInternalUsersAsync(cancellationToken);
}