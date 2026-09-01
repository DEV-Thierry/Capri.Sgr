using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.Common.Interfaces;

public interface IAssociateMembershipStore
{
    Task<AssociateMembership?> FindAsync(Guid id, CancellationToken cancellationToken);
    Task<PrefixProposal?> FindPrefixProposalAsync(Guid id, CancellationToken cancellationToken);
    Task<ReclassificationProposal?> FindOpenReclassificationAsync(Guid membershipId, CancellationToken cancellationToken);
    Task<bool> PrefixIsInUseAsync(string normalizedPrefix, Guid excludingMembershipId, CancellationToken cancellationToken);
    Task AddAsync(AssociateMembership membership, CancellationToken cancellationToken);
    Task AddPrefixProposalAsync(PrefixProposal proposal, CancellationToken cancellationToken);
    Task AddReclassificationProposalAsync(ReclassificationProposal proposal, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}