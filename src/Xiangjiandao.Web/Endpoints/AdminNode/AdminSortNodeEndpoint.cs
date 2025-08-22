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
/// 排序节点
/// </summary>
[Tags("AdminNode")]
[HttpPost("/api/v1/admin/node/sort")]
[Authorize(PolicyNames.Admin)]
public class AdminSortNodeEndpoint(IMediator mediator) : Endpoint<SortNodeReq, ResponseData<bool>>
{
    public override async Task HandleAsync(SortNodeReq req, CancellationToken cancellationToken)
    {
        var command = req.ToCommand();
        await SendAsync(
            response: await mediator.Send(command, cancellationToken).AsSuccessResponseData(),
            cancellation: cancellationToken
        );
    }
}

/// <summary>
/// 排序节点请求
/// </summary>
public class SortNodeReq
{
    /// <summary>
    /// 节点 Id 列表
    /// </summary>
    public required List<NodeId> NodeIds { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public SortNodeCommand ToCommand()
    {
        return new SortNodeCommand
        {
            NodeIds = NodeIds
        };
    }
}