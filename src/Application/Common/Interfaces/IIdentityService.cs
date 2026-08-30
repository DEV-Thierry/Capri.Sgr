using Capri.Sgr.Application.Common.Models;

namespace Capri.Sgr.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);

    Task<IReadOnlyCollection<InternalUser>> GetInternalUsersAsync(CancellationToken cancellationToken);

    Task<(Result Result, string UserId)> CreateInternalUserAsync(string userName, string password, IReadOnlyCollection<string> permissions, CancellationToken cancellationToken);

    Task<Result> UpdateInternalUserPermissionsAsync(string actorId, string userId, IReadOnlyCollection<string> permissions, CancellationToken cancellationToken);
}

public sealed record InternalUser(string Id, string UserName, bool IsActive, IReadOnlyCollection<string> Permissions);
