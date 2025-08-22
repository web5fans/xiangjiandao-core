using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminBanner;
/// <summary>
/// 排序banner
/// </summary>
[Tags("AdminBanners")]
[HttpPost("/api/v1/admin/banner/sort")]
[Authorize(PolicyNames.Admin)]
public class SortBannerEndpoint(
    IMediator mediator): Endpoint<BannerSortReq, ResponseData<bool>>
{
    public override async Task HandleAsync(BannerSortReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync( await mediator.Send(command, ct).AsSuccessResponseData(), cancellation: ct);
    }
}