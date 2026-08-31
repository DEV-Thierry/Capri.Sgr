using Capri.Sgr.Application.Common.Interfaces;

namespace Capri.Sgr.Infrastructure.Services;

/// <summary>Safe default until #10 configures membership types and financial schedules.</summary>
public sealed class NoInitialMembershipChargePolicy : IInitialMembershipChargePolicy
{
    public InitialMembershipCharge? GetFor(string membershipType, DateTimeOffset submittedAt) => null;
}
