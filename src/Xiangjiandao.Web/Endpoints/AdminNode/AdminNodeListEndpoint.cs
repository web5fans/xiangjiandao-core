using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.NodeAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminNode;

/// <summary>
/// 后台节点列表
/// </summary>
[Tags("AdminNode")]
[HttpPost("/api/v1/admin/node/list")]
[Authorize(PolicyNames.Admin)]
public class AdminNodeListEndpoint(NodeQuery query)
    : EndpointWithoutRequest<ResponseData<List<AdminNodeListVo>>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await query.AdminSortedList(ct);
        await SendAsync(response: result.AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 后台列表查询响应
/// </summary>
public class AdminNodeListVo
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

    /// <summary>
    /// 节点用户 Id
    /// </summary>
    public required UserId UserId { get; set; }

    /// <summary>
    /// 节点用户 Did
    /// </summary>
    public required string UserDid { get; set; }

    /// <summary>
    /// 用户手机号
    /// </summary>
    public required string Phone { get; set; }

    /// <summary>
    /// 用户邮箱
    /// </summary>
    public required string Email { get; set; }
}