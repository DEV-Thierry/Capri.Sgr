using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Capri.Sgr.Infrastructure.Services;

public sealed class EfAssociateMembershipStore(ApplicationDbContext context) : IAssociateMembershipStore
{
    public Task<AssociateMembership?> FindAsync(Guid id, CancellationToken ct) => context.AssociateMemberships.SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<PrefixProposal?> FindPrefixProposalAsync(Guid id, CancellationToken ct) => context.PrefixProposals.SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<ReclassificationProposal?> FindOpenReclassificationAsync(Guid membershipId, CancellationToken ct) => context.ReclassificationProposals.SingleOrDefaultAsync(x => x.MembershipId == membershipId && x.Status == ReclassificationProposalStatus.Open, ct);
    public Task<bool> PrefixIsInUseAsync(string normalizedPrefix, Guid excludingMembershipId, CancellationToken ct) => context.AssociateMemberships.AnyAsync(x => x.Id != excludingMembershipId && x.PrefixNormalizedValue == normalizedPrefix, ct);
    public Task AddAsync(AssociateMembership membership, CancellationToken ct) => context.AssociateMemberships.AddAsync(membership, ct).AsTask();
    public Task AddPrefixProposalAsync(PrefixProposal proposal, CancellationToken ct) => context.PrefixProposals.AddAsync(proposal, ct).AsTask();
    public Task AddReclassificationProposalAsync(ReclassificationProposal proposal, CancellationToken ct) => context.ReclassificationProposals.AddAsync(proposal, ct).AsTask();
    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}