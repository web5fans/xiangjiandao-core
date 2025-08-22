using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.NodeAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminNode;

/// <summary>
/// 节点详情查询
/// </summary>
[Tags("AdminNode")]
[HttpPost("/api/v1/admin/node/detail")]
[Authorize(PolicyNames.Admin)]
public class AdminNodeDetailEndpoint(
    NodeQuery query
) : Endpoint<AdminNodeDetailReq, ResponseData<AdminNodeDetailVo>>
{
    public override async Task HandleAsync(AdminNodeDetailReq req, CancellationToken ct)
    {
        var result = await query.AdminDetail(nodeId: req.NodeId, cancellationToken: ct);
        if (result is null)
        {
            throw new KnownException("节点不存在");
        }

        await SendAsync(
            response: result.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 后台节点详情请求
/// </summary>
public class AdminNodeDetailReq
{
    /// <summary>
    /// 节点 Id
    /// </summary>
    public required NodeId NodeId { get; set; }
}

/// <summary>
/// 后台节点详情响应
/// </summary>
public class AdminNodeDetailVo
{
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