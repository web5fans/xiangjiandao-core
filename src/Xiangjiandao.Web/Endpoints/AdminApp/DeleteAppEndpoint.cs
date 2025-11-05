using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.AppAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminApp;

/// <summary>
/// 删除应用 Endpoint
/// </summary>
[Tags("AdminApp")]
[HttpPost("/api/v1/admin/app/delete")]
[Authorize(PolicyNames.Admin)]
public class DeleteAppEndpoint(
    IMediator mediator
) : Endpoint<DeleteAppReq, ResponseData<bool>>
{
    public override async Task HandleAsync(DeleteAppReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        var result = await mediator.Send(command, ct);
        await SendAsync(response: result.AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 删除应用请求
/// </summary>
public class DeleteAppReq
{
    /// <summary>
    /// 应用 Id
    /// </summary>
    public required AppId AppId { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public DeleteAppCommand ToCommand()
    {
        return new DeleteAppCommand
        {
            AppId = AppId,
        };
    }
}