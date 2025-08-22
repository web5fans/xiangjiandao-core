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
[HttpPost("/api/v1/admin/user-manage/user/enable")]
[Authorize(PolicyNames.Admin)]
public class EnableUserEndpoint(
    IMediator mediator): Endpoint<EnableUserReq, ResponseData<bool>>
{
    public override async Task HandleAsync(EnableUserReq req, CancellationToken ct)
    {
        await SendAsync(await mediator.Send(req.ToCommand(), ct)
            .AsSuccessResponseData(), cancellation: ct);
    }
}