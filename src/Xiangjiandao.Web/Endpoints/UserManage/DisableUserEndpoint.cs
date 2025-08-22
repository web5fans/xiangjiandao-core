using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.UserManage;

/// <summary>
/// 禁用用户
/// </summary>
[Tags("UserManages")]
[HttpPost("/api/v1/admin/user-manage/user/disable")]
[Authorize(PolicyNames.Admin)]
public class DisableUserEndpoint(
    IMediator mediator): Endpoint<DisableUserReq, ResponseData<bool>>
{
    public override async Task HandleAsync(DisableUserReq req, CancellationToken ct)
    {
        await SendAsync(await mediator.Send(req.ToCommand(), ct)
            .AsSuccessResponseData(), cancellation: ct);
    }
}