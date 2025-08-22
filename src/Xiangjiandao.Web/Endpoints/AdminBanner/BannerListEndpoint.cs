using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminBanner;

/// <summary>
/// 创建banner
/// </summary>
[Tags("AdminBanners")]
[HttpPost("/api/v1/admin/banner/list")]
[Authorize(PolicyNames.Admin)]
public class BannerListEndpoint(
    BannerQuery query) : EndpointWithoutRequest<ResponseData<List<AdminBannerVo>>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendAsync(await query.List(ct).AsSuccessResponseData(), cancellation: ct);
    }
}