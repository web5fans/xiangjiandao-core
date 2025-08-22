using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.NodeAggregate;

namespace Xiangjiandao.Infrastructure.Repositories;

/// <summary>
/// 节点仓储
/// </summary>
public interface INodeRepository : IRepository<Node, NodeId>
{
    /// <summary>
    /// 通过 Id 列表获取节点列表
    /// </summary>
    Task<List<Node>> GetByIdsAsync(List<NodeId> nodeIds, CancellationToken cancellationToken);

    /// <summary>
    /// 通过名称获取节点
    /// </summary>
    Task<Node?> GetByName(string name, CancellationToken cancellationToken);
}

/// <summary>
/// 节点仓储实现
/// </summary>
public class NodeRepository(ApplicationDbContext context)
    : RepositoryBase<Node, NodeId, ApplicationDbContext>(context), INodeRepository
{
    /// <summary>
    /// 通过 Id 列表获取节点列表
    /// </summary>
    public async Task<List<Node>> GetByIdsAsync(List<NodeId> nodeIds, CancellationToken cancellationToken)
    {
        return await context.Nodes.Where(n => nodeIds.Contains(n.Id)).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 通过名称获取节点
    /// </summary>
    public Task<Node?> GetByName(string name, CancellationToken cancellationToken)
    {
        return context.Nodes.FirstOrDefaultAsync(node => node.Name == name, cancellationToken);
    }
}