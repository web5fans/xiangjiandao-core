using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.NodeAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 排序节点命令
/// </summary>
public class SortNodeCommand : ICommand<bool>
{
    /// <summary>
    /// 节点 Id 列表
    /// </summary>
    public required List<NodeId> NodeIds { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class SortNodeCommandHandler(
    INodeRepository nodeRepository,
    ILogger<SortBannerCommandHandler> logger
) : ICommandHandler<SortNodeCommand, bool>
{
    public async Task<bool> Handle(SortNodeCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("SortNodeCommand Handling");
        var nodes = (await nodeRepository.GetByIdsAsync(
                nodeIds: command.NodeIds,
                cancellationToken: cancellationToken
            ))
            .ToDictionary(keySelector: node => node.Id);

        for (var i = 0; i < command.NodeIds.Count; i++)
        {
            if (nodes.TryGetValue(command.NodeIds[i], out var node))
            {
                node.SetOrder(i);
            }
        }

        return true;
    }
}