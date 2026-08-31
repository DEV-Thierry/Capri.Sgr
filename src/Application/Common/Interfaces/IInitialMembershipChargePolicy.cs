namespace Capri.Sgr.Application.Common.Interfaces;

/// <summary>Provides configured initial charge terms; null means the selected PF membership has no charge.</summary>
public interface IInitialMembershipChargePolicy
{
    InitialMembershipCharge? GetFor(string membershipType, DateTimeOffset submittedAt);
}

public sealed record InitialMembershipCharge(decimal Amount, DateTimeOffset DueAt);
