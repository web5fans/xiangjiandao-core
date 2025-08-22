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
/// Delete 公告
/// </summary>
[Tags("AdminInformation")]
[HttpPost("/api/v1/admin/information/delete")]
[Authorize(PolicyNames.Admin)]
public class AdminDeleteInformationEndpoint(IMediator mediator)
    : Endpoint<DeleteInformationReq, ResponseData<InformationId>>
{
    public override async Task HandleAsync(DeleteInformationReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// Delete 公告请求
/// </summary>
public class DeleteInformationReq
{
    /// <summary>
    /// 公告 Id
    /// </summary>
    public required InformationId InformationId { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public DeleteInformationCommand ToCommand()
    {
        return new DeleteInformationCommand
        {
            InformationId = InformationId,
        };
    }
}
