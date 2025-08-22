using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.BannerAggregate;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminBanner;
/// <summary>
/// 创建banner
/// </summary>
[Tags("AdminBanners")]
[HttpPost("/api/v1/admin/banner/create")]
[Authorize(PolicyNames.Admin)]
public class CreateBannerEndpoint(
    IMediator mediator): Endpoint<BannerCreateReq, ResponseData<BannerId>>
{
    public override async Task HandleAsync(BannerCreateReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync( await mediator.Send(command, ct).AsSuccessResponseData(), cancellation: ct);
    }
}