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
/// 创建节点
/// </summary>
[Tags("AdminNode")]
[HttpPost("/api/v1/admin/node/create")]
[Authorize(PolicyNames.Admin)]
public class AdminCreateNodeEndpoint(IMediator mediator) : Endpoint<CreateNodeReq, ResponseData<NodeId>>
{
    public override async Task HandleAsync(CreateNodeReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 创建节点请求
/// </summary>
public class CreateNodeReq
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
    /// 请求转命令
    /// </summary>
    public CreateNodeCommand ToCommand()
    {
        return new CreateNodeCommand
        {
            UserId = UserId,
            UserDid = UserDid,
            Logo = Logo,
            Name = Name.Trim(),
            Description = Description,
            Sort = 0
        };
    }
}