using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminBanner;
/// <summary>
/// 删除banner
/// </summary>
[Tags("AdminBanners")]
[HttpPost("/api/v1/admin/banner/delete")]
[Authorize(PolicyNames.Admin)]
public class DeleteBannerEndpoint(
    IMediator mediator): Endpoint<BannerDeleteReq, ResponseData<bool>>
{
    public override async Task HandleAsync(BannerDeleteReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync( await mediator.Send(command, ct).AsSuccessResponseData(), cancellation: ct);
    }
}