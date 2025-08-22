using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.NodeAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminNode;

/// <summary>
/// 修改节点
/// </summary>
[Tags("AdminNode")]
[HttpPost("/api/v1/admin/node/modify")]
[Authorize(PolicyNames.Admin)]
public class AdminModifyNodeEndpoint(IMediator mediator) : Endpoint<ModifyNodeReq, ResponseData<bool>>
{
    public override async Task HandleAsync(ModifyNodeReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 修改节点请求
/// </summary>
public class ModifyNodeReq
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

    /// <summary>
    /// 请求转命令
    /// </summary>
    public ModifyNodeCommand ToCommand()
    {
        return new ModifyNodeCommand
        {
            UserId = UserId,
            UserDid = UserDid,
            NodeId = NodeId,
            Logo = Logo,
            Name = Name,
            Description = Description,
        };
    }
}