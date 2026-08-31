using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.AssociateMemberships;

/// <summary>Coordinates internal Afixo decisions and idempotent, supplied-count reenquadramento evaluation.</summary>
public sealed class AssociateMembershipService(IAssociateMembershipStore memberships, IAuditStore audit, IOperationalClock clock, IUser user)
{
    public async Task<Guid> ProposePrefixAsync(Guid membershipId, PrefixKind? kind, string? value, CancellationToken cancellationToken)
    {
        var membership = await RequireMembership(membershipId, cancellationToken);
        if (!AssociateTypeCatalogue.Get(membership.Type).AllowsPrefix) throw new InvalidOperationException("This associate type does not permit an Afixo.");
        var proposal = new PrefixProposal(membershipId, kind, value, clock.GetCurrentInstant());
        if (proposal.NormalizedValue is not null && await memberships.PrefixIsInUseAsync(proposal.NormalizedValue, membershipId, cancellationToken)) throw new InvalidOperationException("Afixo is already in use.");
        await memberships.AddPrefixProposalAsync(proposal, cancellationToken); await memberships.SaveChangesAsync(cancellationToken);
        await AuditAsync("Proposta de Afixo criada", proposal.Id.ToString(), null, proposal.NormalizedValue, cancellationToken); return proposal.Id;
    }
    public async Task ApprovePrefixAsync(Guid proposalId, CancellationToken cancellationToken)
    {
        var proposal = await memberships.FindPrefixProposalAsync(proposalId, cancellationToken) ?? throw new KeyNotFoundException("Proposta de Afixo não encontrada.");
        if (proposal.Status != PrefixProposalStatus.Open) throw new InvalidOperationException("Only an open Afixo proposal can be approved.");
        var membership = await RequireMembership(proposal.MembershipId, cancellationToken);
        if (proposal.NormalizedValue is not null && await memberships.PrefixIsInUseAsync(proposal.NormalizedValue, membership.Id, cancellationToken)) throw new InvalidOperationException("Afixo is already in use.");
        var before = membership.PrefixNormalizedValue; proposal.Approve(); membership.ApplyApprovedPrefix(proposal); await memberships.SaveChangesAsync(cancellationToken);
        await AuditAsync("Proposta de Afixo aprovada", proposal.Id.ToString(), before, membership.PrefixNormalizedValue, cancellationToken);
    }
    public async Task EvaluateReclassificationAsync(Guid membershipId, int activeHerdCount, CancellationToken cancellationToken)
    {
        if (activeHerdCount < 0) throw new ArgumentOutOfRangeException(nameof(activeHerdCount));
        var membership = await RequireMembership(membershipId, cancellationToken);
        if (clock.GetOperationalDate().DayNumber - membership.CreatedOn.DayNumber <= 30) return;
        var outcome = GetOutcome(membership.Type, activeHerdCount);
        var open = await memberships.FindOpenReclassificationAsync(membershipId, cancellationToken);
        if (outcome.IsCurrentRange) { if (open is not null) { open.Cancel(); await memberships.SaveChangesAsync(cancellationToken); await AuditAsync("Proposta de reenquadramento cancelada", open.Id.ToString(), null, null, cancellationToken); } return; }
        var destination = outcome.Destination;
        if (open is not null) { open.Update(destination, activeHerdCount, clock.GetCurrentInstant()); await memberships.SaveChangesAsync(cancellationToken); return; }
        var proposal = new ReclassificationProposal(membershipId, destination, activeHerdCount, clock.GetCurrentInstant()); await memberships.AddReclassificationProposalAsync(proposal, cancellationToken); await memberships.SaveChangesAsync(cancellationToken);
        await AuditAsync("Proposta de reenquadramento criada", proposal.Id.ToString(), membership.Type.ToString(), destination?.ToString(), cancellationToken);
    }
    private static ReclassificationOutcome GetOutcome(AssociateType current, int herd) => current switch
    {
        AssociateType.ContributingJunior when herd >= 60 => new(AssociateType.ContributingSenior, false),
        AssociateType.ContributingJunior when herd is >= 6 and <= 59 => new(current, true),
        AssociateType.ContributingSenior when herd is >= 6 and <= 59 => new(AssociateType.ContributingJunior, false),
        AssociateType.ContributingSenior when herd >= 60 => new(current, true),
        _ => new(null, false)
    };
    private sealed record ReclassificationOutcome(AssociateType? Destination, bool IsCurrentRange);
    private async Task<AssociateMembership> RequireMembership(Guid id, CancellationToken ct) => await memberships.FindAsync(id, ct) ?? throw new KeyNotFoundException("Associado não encontrado.");
    private Task AuditAsync(string action, string entityId, string? before, string? after, CancellationToken ct) => audit.AppendAsync(new AuditRecord(user.Id, clock.GetCurrentInstant(), "internal", "Associado", entityId, action, null, before, after), ct);
}