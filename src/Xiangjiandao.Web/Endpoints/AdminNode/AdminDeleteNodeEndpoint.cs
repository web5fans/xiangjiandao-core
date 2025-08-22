using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.NodeAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminNode;

/// <summary>
/// 删除节点
/// </summary>
[Tags("AdminNode")]
[HttpPost("/api/v1/admin/node/delete")]
[Authorize(PolicyNames.Admin)]
public class AdminDeleteNodeEndpoint(IMediator mediator) : Endpoint<DeleteNodeReq, ResponseData<bool>>
{
    public override async Task HandleAsync(DeleteNodeReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 删除节点请求
/// </summary>
public class DeleteNodeReq
{
    /// <summary>
    /// 节点 Id
    /// </summary>
    public required NodeId NodeId { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public DeleteNodeCommand ToCommand()
    {
        return new DeleteNodeCommand
        {
            NodeId = NodeId,       
        };
    }
}