using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.NodeAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 删除节点请求
/// </summary>
public class DeleteNodeCommand : ICommand<bool>
{
    /// <summary>
    /// 节点 Id
    /// </summary>
    public required NodeId NodeId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class DeleteNodeCommandHandler(
    INodeRepository nodeRepository,
    ILogger<DeleteNodeCommandHandler> logger
) : ICommandHandler<DeleteNodeCommand, bool>
{
    public async Task<bool> Handle(DeleteNodeCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("DeleteNodeCommand Handling");
        var node = await nodeRepository.GetAsync(command.NodeId, cancellationToken);
        if (node is null)
        {
            throw new KnownException("节点不存在");
        }

        node.Delete();

        return true;
    }
}