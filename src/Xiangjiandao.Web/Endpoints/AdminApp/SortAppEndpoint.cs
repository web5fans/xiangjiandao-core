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
/// 排序应用 Endpoint
/// </summary>
[Tags("AdminApp")]
[HttpPost("/api/v1/admin/app/sort")]
[Authorize(PolicyNames.Admin)]
public class SortAppEndpoint(
    IMediator mediator
) : Endpoint<SortAppReq, ResponseData<bool>>
{
    public override async Task HandleAsync(SortAppReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        var result = await mediator.Send(command, ct);
        await SendAsync(response: result.AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 排序应用请求
/// </summary>
public class SortAppReq
{
    /// <summary>
    /// 应用 Id 列表
    /// </summary>
    public required List<AppId> AppIds { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public SortAppCommand ToCommand()
    {
        return new SortAppCommand
        {
            AppIds = AppIds,
        };
    }
}