using System.Security.Claims;
using System.Text.Json;
using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Application.Common.Models;
using Capri.Sgr.Domain.Constants;
using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Capri.Sgr.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly ApplicationDbContext _context;
    private readonly IAuditStore _auditStore;
    private readonly IOperationalClock _clock;

    public IdentityService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, IAuthorizationService authorizationService, ApplicationDbContext context, IAuditStore auditStore, IOperationalClock clock)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _context = context;
        _auditStore = auditStore;
        _clock = clock;
    }

    public async Task<string?> GetUserNameAsync(string userId) => (await _userManager.FindByIdAsync(userId))?.UserName;

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser { UserName = userName, Email = userName };
        var result = await _userManager.CreateAsync(user, password);
        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        return (await _authorizationService.AuthorizeAsync(principal, policyName)).Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user) => (await _userManager.DeleteAsync(user)).ToApplicationResult();

    public async Task<IReadOnlyCollection<InternalUser>> GetInternalUsersAsync(CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.OrderBy(user => user.UserName).ToListAsync(cancellationToken);
        var result = new List<InternalUser>(users.Count);
        foreach (var user in users)
        {
            var permissions = await GetPermissionsAsync(user);
            result.Add(new InternalUser(user.Id, user.UserName ?? user.Email ?? user.Id, !user.LockoutEnd.HasValue || user.LockoutEnd <= DateTimeOffset.UtcNow, permissions));
        }
        return result;
    }

    public async Task<(Result Result, string UserId)> CreateInternalUserAsync(string userName, string password, IReadOnlyCollection<string> permissions, CancellationToken cancellationToken)
    {
        if (!AreKnownPermissions(permissions)) return (Result.Failure(["Uma ou mais permissões administrativas são desconhecidas."]), string.Empty);
        var user = new ApplicationUser { UserName = userName, Email = userName };
        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded) return (createResult.ToApplicationResult(), string.Empty);
        var claimResult = await ReplacePermissionsAsync(user, permissions);
        if (!claimResult.Succeeded) return (claimResult, string.Empty);
        await AppendAuditAsync(null, user.Id, "Created", null, permissions, cancellationToken);
        return (Result.Success(), user.Id);
    }

    public async Task<Result> UpdateInternalUserPermissionsAsync(string actorId, string userId, IReadOnlyCollection<string> permissions, CancellationToken cancellationToken)
    {
        if (!AreKnownPermissions(permissions)) return Result.Failure(["Uma ou mais permissões administrativas são desconhecidas."]);
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result.Failure(["Usuário interno não encontrado."]);
        var before = await GetPermissionsAsync(user);
        if (actorId == userId && before.Except(permissions).Any())
            return Result.Failure(["Um Usuário interno não pode revogar a própria permissão administrativa na mesma ação."]);

        var removesAdministratorAccess = await _userManager.IsInRoleAsync(user, Roles.Administrator) &&
            !permissions.Contains(AdministrativePermission.Create(AdministrativeResources.InternalUsers, AdministrativeActions.Configure));
        if (removesAdministratorAccess && await ActiveAdministratorCountAsync() == 1)
            return Result.Failure(["Não é permitido remover o acesso administrativo do último Administrador ativo."]);

        var result = await ReplacePermissionsAsync(user, permissions);
        if (!result.Succeeded) return result;
        await AppendAuditAsync(actorId, user.Id, "PermissionsUpdated", before, permissions, cancellationToken);
        return Result.Success();
    }

    private async Task<IReadOnlyCollection<string>> GetPermissionsAsync(ApplicationUser user) =>
        (await _userManager.GetClaimsAsync(user)).Where(claim => claim.Type == AdministrativePermission.ClaimType).Select(claim => claim.Value).Order().ToArray();

    private async Task<Result> ReplacePermissionsAsync(ApplicationUser user, IReadOnlyCollection<string> permissions)
    {
        var existing = (await _userManager.GetClaimsAsync(user)).Where(claim => claim.Type == AdministrativePermission.ClaimType).ToArray();
        var remove = await _userManager.RemoveClaimsAsync(user, existing);
        if (!remove.Succeeded) return remove.ToApplicationResult();
        return (await _userManager.AddClaimsAsync(user, permissions.Distinct().Select(permission => new Claim(AdministrativePermission.ClaimType, permission)))).ToApplicationResult();
    }

    private async Task<int> ActiveAdministratorCountAsync()
    {
        var administrators = await _userManager.GetUsersInRoleAsync(Roles.Administrator);
        return administrators.Count(user => !user.LockoutEnd.HasValue || user.LockoutEnd <= DateTimeOffset.UtcNow);
    }

    private static bool AreKnownPermissions(IEnumerable<string> permissions) => permissions.All(permission => AdministrativePermission.All.Contains(permission));

    private Task AppendAuditAsync(string? actorId, string userId, string action, IReadOnlyCollection<string>? before, IReadOnlyCollection<string> after, CancellationToken cancellationToken) =>
        _auditStore.AppendAsync(new AuditRecord(actorId, _clock.GetCurrentInstant(), "web", "Usuário interno", userId, action, null, before is null ? null : JsonSerializer.Serialize(before), JsonSerializer.Serialize(after)), cancellationToken);
}