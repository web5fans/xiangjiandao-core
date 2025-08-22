using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.Proposal;

/// <summary>
/// 投票提案
/// </summary>
[Tags("Proposal")]
[HttpPost("/api/v1/proposal/vote")]
[Authorize(PolicyNames.Client)]
public class VoteProposalEndpoint(
    IMediator mediator,
    ILoginUser loginUser
) : Endpoint<VoteProposalReq, ResponseData<bool>>
{
    public override async Task HandleAsync(VoteProposalReq req, CancellationToken ct)
    {
        var userId = new UserId(loginUser.Id);
        var command = req.ToCommand(userId);
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 投票提案请求
/// </summary>
public class VoteProposalReq
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }

    /// <summary>
    /// 投票选择
    /// </summary>
    public required VoteType Choose { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public VoteProposalCommand ToCommand(UserId userId)
    {
        return new VoteProposalCommand
        {
            ProposalId = ProposalId,
            UserId = userId,
            Choose = Choose
        };
    }
}