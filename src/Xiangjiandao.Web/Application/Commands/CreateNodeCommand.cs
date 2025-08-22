using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.NodeAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 创建 Node 命令
/// </summary>
public class CreateNodeCommand : ICommand<NodeId>
{
    /// <summary>
    /// 节点用户 Id
    /// </summary>
    public required UserId UserId { get; set; }

    /// <summary>
    /// 节点用户 Did
    /// </summary>
    public required string UserDid { get; set; }

    /// <summary>
    /// 节点 Logo
    /// </summary> 
    public required string Logo { get; set; }

    /// <summary>
    /// 节点名称
    /// </summary> 
    public required string Name { get; set; }

    /// <summary>
    /// 节点描述
    /// </summary> 
    public required string Description { get; set; }

    /// <summary>
    /// 排序
    /// </summary> 
    public required int Sort { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class CreateNodeCommandHandler(
    INodeRepository nodeRepository,
    ILogger<CreateNodeCommandHandler> logger
) : ICommandHandler<CreateNodeCommand, NodeId>
{
    public async Task<NodeId> Handle(CreateNodeCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("CreateNodeCommand Handling");

        var existNode = await nodeRepository.GetByName(
            name: command.Name,
            cancellationToken: cancellationToken
        );
        if (existNode is not null)
        {
            throw new KnownException("该节点已存在");
        }

        var node = Node.Create(
            userId: command.UserId,
            userDid: command.UserDid,
            logo: command.Logo,
            name: command.Name,
            description: command.Description,
            sort: command.Sort
        );
        await nodeRepository.AddAsync(node, cancellationToken);
        return node.Id;
    }
}