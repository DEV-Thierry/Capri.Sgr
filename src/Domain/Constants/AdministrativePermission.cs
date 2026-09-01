namespace Capri.Sgr.Domain.Constants;

/// <summary>Names the administrative resources that can be independently authorized.</summary>
public static class AdministrativeResources
{
    public const string InternalUsers = "internal-users";
    public const string InstitutionalPublications = "institutional-publications";

    public static IReadOnlyCollection<string> All { get; } = [InternalUsers, InstitutionalPublications];
}

/// <summary>Names the independently grantable operations for an administrative resource.</summary>
public static class AdministrativeActions
{
    public const string Consult = "consult";
    public const string Maintain = "maintain";
    public const string Decide = "decide";
    public const string Configure = "configure";

    public static IReadOnlyCollection<string> All { get; } = [Consult, Maintain, Decide, Configure];
}

/// <summary>Builds canonical policy and claim values for a resource/action administrative permission.</summary>
public static class AdministrativePermission
{
    public const string ClaimType = "capri:administrative-permission";
    public const string PolicyPrefix = "administrative:";

    public static string Create(string resource, string action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resource);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        return $"{resource}:{action}";
    }

    public static string Policy(string resource, string action) => PolicyPrefix + Create(resource, action);

    public static IReadOnlyCollection<string> All { get; } =
        AdministrativeResources.All.SelectMany(resource => AdministrativeActions.All.Select(action => Create(resource, action))).ToArray();
}
