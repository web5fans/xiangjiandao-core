using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Infrastructure.Repositories;

public interface IProposalRepository : IRepository<Proposal, ProposalId>
{
    /// <summary>
    /// 通过用户 Id 查询提案
    /// </summary>
    Task<List<Proposal>> GetByUserId(UserId userId, CancellationToken cancellationToken);
}

public class ProposalRepository(ApplicationDbContext context)
    : RepositoryBase<Proposal, ProposalId, ApplicationDbContext>(context), IProposalRepository
{
    /// <summary>
    /// 通过用户 Id 查询提案
    /// </summary>
    public async Task<List<Proposal>> GetByUserId(UserId userId, CancellationToken cancellationToken)
    {
        return await context.Proposals
            .Where(proposal => proposal.InitiatorId == userId)
            .ToListAsync(cancellationToken);
    }
}