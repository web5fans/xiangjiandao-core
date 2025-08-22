using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminBanner;
/// <summary>
/// 编辑banner
/// </summary>
[Tags("AdminBanners")]
[HttpPost("/api/v1/admin/banner/modify")]
[Authorize(PolicyNames.Admin)]
public class ModifyBannerEndpoint(
    IMediator mediator): Endpoint<BannerModifyReq, ResponseData<bool>>
{
    public override async Task HandleAsync(BannerModifyReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync( await mediator.Send(command, ct).AsSuccessResponseData(), cancellation: ct);
    }
}