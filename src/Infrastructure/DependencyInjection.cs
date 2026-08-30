using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Infrastructure.Data;
using Capri.Sgr.Domain.Constants;
using Capri.Sgr.Infrastructure.Data.Interceptors;
using Capri.Sgr.Infrastructure.Identity;
using Capri.Sgr.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(Services.Database);
        Guard.Against.Null(connectionString, message: $"Connection string '{Services.Database}' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(AdministrativePermission.Policy(AdministrativeResources.InternalUsers, AdministrativeActions.Consult), policy => policy.RequireClaim(AdministrativePermission.ClaimType, AdministrativePermission.Create(AdministrativeResources.InternalUsers, AdministrativeActions.Consult)))
            .AddPolicy(AdministrativePermission.Policy(AdministrativeResources.InternalUsers, AdministrativeActions.Maintain), policy => policy.RequireClaim(AdministrativePermission.ClaimType, AdministrativePermission.Create(AdministrativeResources.InternalUsers, AdministrativeActions.Maintain)))
            .AddPolicy(AdministrativePermission.Policy(AdministrativeResources.InternalUsers, AdministrativeActions.Decide), policy => policy.RequireClaim(AdministrativePermission.ClaimType, AdministrativePermission.Create(AdministrativeResources.InternalUsers, AdministrativeActions.Decide)))
            .AddPolicy(AdministrativePermission.Policy(AdministrativeResources.InternalUsers, AdministrativeActions.Configure), policy => policy.RequireClaim(AdministrativePermission.ClaimType, AdministrativePermission.Create(AdministrativeResources.InternalUsers, AdministrativeActions.Configure)));

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<IOperationalClock, OperationalClock>();
        builder.Services.AddScoped<IAuditStore, EfAuditStore>();
        builder.Services.AddScoped<IDerivedEffectExecutionStore, EfDerivedEffectExecutionStore>();
        builder.Services.AddTransient<IIdentityService, IdentityService>();
    }
}