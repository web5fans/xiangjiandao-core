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
/// Create 公告
/// </summary>
[Tags("AdminInformation")]
[HttpPost("/api/v1/admin/information/create")]
[Authorize(PolicyNames.Admin)]
public class AdminCreateInformationEndpoint(IMediator mediator)
    : Endpoint<CreateInformationReq, ResponseData<InformationId>>
{
    public override async Task HandleAsync(CreateInformationReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// Create 公告请求
/// </summary>
public class CreateInformationReq
{
    /// <summary>
    /// 公告名称
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 附件 Id
    /// </summary>
    public required string AttachId { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public CreateInformationCommand ToCommand()
    {
        return new CreateInformationCommand
        {
            Name = Name,
            AttachId = AttachId
        };
    }
}