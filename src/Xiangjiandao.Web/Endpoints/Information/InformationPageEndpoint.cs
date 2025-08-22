using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;

namespace Xiangjiandao.Web.Endpoints.Information;

/// <summary>
/// 公告分页查询
/// </summary>
[Tags("Information")]
[HttpPost("/api/v1/information/page")]
[AllowAnonymous]
public class InformationPageEndpoint(
    InformationQuery query
) : Endpoint<InformationPageReq, ResponseData<PagedData<InformationPageVo>>>
{
    public override async Task HandleAsync(InformationPageReq req, CancellationToken ct)
    {
        var result = await query.Page(pageIndex: req.PageNum, pageSize: req.PageSize, cancellationToken: ct);
        await SendAsync(
            response: result.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 公告分页查询响应
/// </summary>
public class InformationPageVo
{
    /// <summary>
    /// 公告 Id
    /// </summary>
    public required InformationId Id { get; set; }

    /// <summary>
    /// 公告名称
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 附件 Id
    /// </summary>
    public required string AttachId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public required DateTime CreateAt { get; set; }
}

/// <summary>
/// 公告分页查询请求
/// </summary>
public class InformationPageReq
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
}