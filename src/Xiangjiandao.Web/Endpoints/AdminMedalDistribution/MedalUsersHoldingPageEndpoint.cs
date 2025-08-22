using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminMedalDistribution;

/// <summary>
/// 勋章的持有用户翻页查询
/// </summary>
[Tags("MedalDistributions")]
[HttpPost("/api/v1/admin/medal/users-holding/page")]
[Authorize(PolicyNames.AdminOnly)]
public class MedalUsersHoldingPageEndpoint(
    UserMedalQuery query): Endpoint<AdminUserMedalPageReq, ResponseData<PagedData<AdminUserMedalPageVo>>>
{
    public override async Task HandleAsync(AdminUserMedalPageReq req, CancellationToken ct)
    {
        
        await SendAsync( await query.Page(req, ct).AsSuccessResponseData(), cancellation: ct);
    }
}