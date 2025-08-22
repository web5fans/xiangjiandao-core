using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminScoreDistribution;

/// <summary>
/// 稻米发放翻页查询
/// </summary>
[Tags("ScoreDistributions")]
[HttpPost("/api/v1/admin/score-distribute-record/page")]
[Authorize(PolicyNames.AdminOnly)]
public class ScoreDistributeRecordPageEndpoint(
    ScoreDistributeRecordQuery query): Endpoint<AdminScoreDistributeRecordPageReq, ResponseData<PagedData<AdminScoreDistributeRecordPageVo>>>
{
    public override async Task HandleAsync(AdminScoreDistributeRecordPageReq req, CancellationToken ct)
    {
        
        await SendAsync( await query.Page(req, ct).AsSuccessResponseData(), cancellation: ct);
    }
}