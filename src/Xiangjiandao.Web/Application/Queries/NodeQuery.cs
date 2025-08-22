using Microsoft.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.NodeAggregate;
using Xiangjiandao.Web.Endpoints.AdminNode;
using Xiangjiandao.Web.Endpoints.Node;

namespace Xiangjiandao.Web.Application.Queries;

/// <summary>
/// 节点相关查询
/// </summary>
public class NodeQuery(
    ApplicationDbContext dbContext,
    ILogger<NodeQuery> logger
)
{
    /// <summary>
    /// 后台节点详情
    /// </summary>
    public async Task<AdminNodeDetailVo?> AdminDetail(NodeId nodeId, CancellationToken cancellationToken)
    {
        logger.LogInformation("NodeQuery::AdminNodeDetail");
        return await dbContext.Nodes
            .Where(node => node.Id == nodeId)
            .Select(node => new AdminNodeDetailVo
            {
                NodeId = node.Id,
                Logo = node.Logo,
                Name = node.Name,
                Description = node.Description
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 后台排序列表
    /// </summary>
    public async Task<List<AdminNodeListVo>> AdminSortedList(CancellationToken cancellationToken)
    {
        logger.LogInformation("NodeQuery::AdminNodeSortedList");
        var result = await dbContext.Nodes
            .OrderBy(node => node.Sort)
            .ThenByDescending(node => node.CreatedAt)
            .Select(node => new AdminNodeListVo
            {
                NodeId = node.Id,
                Logo = node.Logo,
                Name = node.Name,
                Description = node.Description,
                UserId = node.UserId,
                UserDid = node.UserDid,
                Phone = string.Empty,
                Email = string.Empty
            })
            .ToListAsync(cancellationToken: cancellationToken);
        var scoreDict = await dbContext.Users
            .Where(user => result.Select(node => node.UserId).Contains(user.Id))
            .Select(user => new { user.Id, user.Phone, user.Email, })
            .ToDictionaryAsync(
                keySelector: item => item.Id,
                cancellationToken: cancellationToken
            );

        result.ForEach(node =>
            {
                if (!scoreDict.TryGetValue(node.UserId, out var score))
                {
                    return;
                }
                node.Phone = score.Phone;
                node.Email = score.Email;
            }
        );

        return result;
    }

    /// <summary>
    /// 排序列表
    /// </summary>
    public async Task<List<NodeListVo>> SortedList(CancellationToken cancellationToken)
    {
        logger.LogInformation("NodeQuery::NodeSortedList");
        var result = await dbContext.Nodes
            .OrderBy(node => node.Sort)
            .ThenByDescending(node => node.CreatedAt)
            .Select(node => new NodeListVo
            {
                NodeId = node.Id,
                Logo = node.Logo,
                Name = node.Name,
                Description = node.Description,
                UserId = node.UserId,
                UserDid = node.UserDid,
                Score = 0
            })
            .ToListAsync(cancellationToken: cancellationToken);

        var scoreDict = await dbContext.Users
            .Where(user => result.Select(node => node.UserId).Contains(user.Id))
            .Select(user => new { user.Id, user.Score, })
            .ToDictionaryAsync(
                keySelector: item => item.Id,
                elementSelector: item => item.Score,
                cancellationToken: cancellationToken
            );
        result.ForEach(node => node.Score = scoreDict.TryGetValue(node.UserId, out var score) ? score : 0);

        return result;
    }
}