using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.NodeAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 修改节点命令
/// </summary>
public class ModifyNodeCommand : ICommand<bool>
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
    /// 节点 Id
    /// </summary>
    public required NodeId NodeId { get; set; }

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
}

/// <summary>
/// 命令处理器
/// </summary>
public class ModifyNodeCommandHandler(
    INodeRepository nodeRepository,
    ILogger<ModifyNodeCommandHandler> logger
) : ICommandHandler<ModifyNodeCommand, bool>
{
    public async Task<bool> Handle(ModifyNodeCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("ModifyNodeCommand Handling");
        var node = await nodeRepository.GetAsync(command.NodeId, cancellationToken);
        if (node is null)
        {
            throw new KnownException("节点不存在");
        }

        node.Modify(
            userId: command.UserId,
            userDid: command.UserDid,
            logo: command.Logo,
            name: command.Name,
            description: command.Description
        );
        
        return true;
    }
}