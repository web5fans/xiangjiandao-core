using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminMedalDistribution;

/// <summary>
/// 勋章翻页查询
/// </summary>
[Tags("MedalDistributions")]
[HttpPost("/api/v1/admin/medal/page")]
[Authorize(PolicyNames.AdminOnly)]
public class MedalPageEndpoint(
    MedalQuery query): Endpoint<AdminMedalPageReq, ResponseData<PagedData<AdminMedalPageVo>>>
{
    public override async Task HandleAsync(AdminMedalPageReq req, CancellationToken ct)
    {
        
        await SendAsync( await query.Page(req, ct).AsSuccessResponseData(), cancellation: ct);
    }
}

public record AdminMedalPageReq
{
    /// <summary>
    ///  分页页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// 分页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
}