using System.Security.Claims;
using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Constants;
using Capri.Sgr.Infrastructure.Data;
using Capri.Sgr.Infrastructure.Identity;
using Capri.Sgr.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Infrastructure.IntegrationTests;

public class InternalUserPermissionTests
{
    [Test]
    public async Task LastActiveAdministratorCannotLoseAdministrativeConfigurationAccess()
    {
        await using var provider = CreateProvider();
        using var scope = provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await roleManager.CreateAsync(new IdentityRole(Roles.Administrator));
        var administrator = new ApplicationUser { UserName = "admin@local", Email = "admin@local" };
        (await userManager.CreateAsync(administrator, "Administrator1234!")).Succeeded.ShouldBeTrue();
        await userManager.AddToRoleAsync(administrator, Roles.Administrator);
        await userManager.AddClaimAsync(administrator, new Claim(AdministrativePermission.ClaimType, AdministrativePermission.Create(AdministrativeResources.InternalUsers, AdministrativeActions.Configure)));
        var service = scope.ServiceProvider.GetRequiredService<IIdentityService>();

        var result = await service.UpdateInternalUserPermissionsAsync("another-admin", administrator.Id, [], CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
        result.Errors.Single().ShouldContain("último Administrador ativo");
        (await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().AuditRecords.CountAsync()).ShouldBe(0);
    }

    [Test]
    public async Task UserCannotRevokeOwnPermissionAndSuccessfulChangeIsAudited()
    {
        await using var provider = CreateProvider();
        using var scope = provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = new ApplicationUser { UserName = "operator@local", Email = "operator@local" };
        (await userManager.CreateAsync(user, "Operator1234!")).Succeeded.ShouldBeTrue();
        var consult = AdministrativePermission.Create(AdministrativeResources.InternalUsers, AdministrativeActions.Consult);
        var maintain = AdministrativePermission.Create(AdministrativeResources.InternalUsers, AdministrativeActions.Maintain);
        await userManager.AddClaimsAsync(user, [new Claim(AdministrativePermission.ClaimType, consult), new Claim(AdministrativePermission.ClaimType, maintain)]);
        var service = scope.ServiceProvider.GetRequiredService<IIdentityService>();

        var selfResult = await service.UpdateInternalUserPermissionsAsync(user.Id, user.Id, [consult], CancellationToken.None);
        selfResult.Succeeded.ShouldBeFalse();
        var updateResult = await service.UpdateInternalUserPermissionsAsync("administrator", user.Id, [consult], CancellationToken.None);

        updateResult.Succeeded.ShouldBeTrue();
        var audit = await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().AuditRecords.SingleAsync();
        audit.ActorId.ShouldBe("administrator");
        audit.EntityType.ShouldBe("Usuário interno");
        audit.Action.ShouldBe("PermissionsUpdated");
        audit.Before.ShouldNotBeNull();
        audit.After.ShouldNotBeNull();
        audit.Before.ShouldContain(maintain);
        audit.After.ShouldContain(consult);
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization();
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddScoped<IAuditStore, EfAuditStore>();
        services.AddSingleton<IOperationalClock>(new TestClock());
        services.AddScoped<IIdentityService, IdentityService>();
        return services.BuildServiceProvider();
    }

    private sealed class TestClock : IOperationalClock
    {
        public DateTimeOffset GetCurrentInstant() => new(2026, 4, 1, 9, 30, 0, TimeSpan.FromHours(-3));
        public DateOnly GetOperationalDate() => new(2026, 4, 1);
    }
}