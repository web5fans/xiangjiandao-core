using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.UserManage;

/// <summary>
/// 设置节点用户
/// </summary>
[Tags("UserManages")]
[HttpPost("/api/v1/admin/user-manage/user/set-node-user")]
[Authorize(PolicyNames.Admin)]
public class SetNodeUserEndpoint(
    IMediator mediator): Endpoint<SetNodeUserReq, ResponseData<bool>>
{
    public override async Task HandleAsync(SetNodeUserReq req, CancellationToken ct)
    {
        await SendAsync(await mediator.Send(req.ToCommand(), ct)
            .AsSuccessResponseData(), cancellation: ct);
    }
}