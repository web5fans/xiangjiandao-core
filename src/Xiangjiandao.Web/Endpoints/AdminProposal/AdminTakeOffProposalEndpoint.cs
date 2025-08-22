using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminProposal;

/// <summary>
/// 后台下架
/// </summary>
[Tags("AdminProposal")]
[HttpPost("/api/v1/admin/proposal/take-off")]
[Authorize(PolicyNames.Admin)]
public class AdminTakeOffProposalEndpoint(IMediator mediator) : Endpoint<AdminTakeOffProposalReq, ResponseData<bool>>
{
    public override async Task HandleAsync(AdminTakeOffProposalReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 后台下架提案请求
/// </summary>
public class AdminTakeOffProposalReq
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public TakeOffProposalCommand ToCommand()
    {
        return new TakeOffProposalCommand
        {
            ProposalId = ProposalId,
        };
    }
}