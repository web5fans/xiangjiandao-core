using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.NodeAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;

namespace Xiangjiandao.Web.Endpoints.Node;

/// <summary>
/// 节点列表
/// </summary>
[Tags("Node")]
[HttpPost("/api/v1/node/list")]
[AllowAnonymous]
public class NodeListEndpoint(NodeQuery query) : EndpointWithoutRequest<ResponseData<List<NodeListVo>>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await query.SortedList(cancellationToken: ct);
        await SendAsync(response: result.AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 节点列表响应
/// </summary>
public class NodeListVo
{
    /// <summary>
    /// 节点 Id
    /// </summary>
    public required NodeId NodeId { get; set; }

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
    /// 稻米数量
    /// </summary>
    public required long Score { get; set; }
}