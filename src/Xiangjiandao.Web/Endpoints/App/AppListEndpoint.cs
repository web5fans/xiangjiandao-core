using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.AppAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;

namespace Xiangjiandao.Web.Endpoints.App;

/// <summary>
/// 节点列表
/// </summary>
[Tags("App")]
[HttpPost("/api/v1/app/list")]
[AllowAnonymous]
public class AppListEndpoint(AppQuery query) : EndpointWithoutRequest<ResponseData<List<AppListVo>>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await query.SortedList(cancellationToken: ct);
        await SendAsync(response: result.AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 应用列表响应
/// </summary>
public class AppListVo
{
    /// <summary>
    /// 应用名称
    /// </summary>
    public required AppId AppId { get; set; }

    /// <summary>
    /// 应用名称
    /// </summary> 
    public required string Name { get; set; }

    /// <summary>
    /// 应用描述
    /// </summary> 
    public required string Desc { get; set; }

    /// <summary>
    /// 应用图标
    /// </summary> 
    public required string Logo { get; set; }

    /// <summary>
    /// 应用链接
    /// </summary> 
    public required string Link { get; set; }
}