using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;

namespace Xiangjiandao.Web.Endpoints.Banner;
/// <summary>
/// 查询banner列表
/// </summary>
[Tags("Banners")]
[HttpPost("/api/v1/banner/list")]
[AllowAnonymous]
public class BannerListForCEndpoint(
    BannerQuery query): EndpointWithoutRequest<ResponseData<List<BannerVo>>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        
        await SendAsync( await query.ListForC(ct).AsSuccessResponseData(), cancellation: ct);
    }
}