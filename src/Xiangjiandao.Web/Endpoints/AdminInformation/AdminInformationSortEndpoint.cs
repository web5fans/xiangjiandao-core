using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminInformation;

/// <summary>
/// 公告排序
/// </summary>
[Tags("AdminInformation")]
[HttpPost("/api/v1/admin/information/sort")]
[Authorize(PolicyNames.Admin)]
public class AdminSortInformationEndpoint(IMediator mediator)
    : Endpoint<SortInformationReq, ResponseData<bool>>
{
    public override async Task HandleAsync(SortInformationReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// Sort 公告请求
/// </summary>
public class SortInformationReq
{
    /// <summary>
    /// 公告 Id 列表
    /// </summary>
    public required List<InformationId> InformationIds { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public SortInformationCommand ToCommand()
    {
        return new SortInformationCommand
        {
            InformationIds = InformationIds,
        };
    }
}