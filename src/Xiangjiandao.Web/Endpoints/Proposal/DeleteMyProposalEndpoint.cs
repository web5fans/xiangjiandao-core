using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.Proposal;

/// <summary>
/// 删除我的提案
/// </summary>
[Tags("Proposal")]
[HttpPost("/api/v1/proposal/delete-my-proposal")]
[Authorize(PolicyNames.Client)]
public class DeleteMyProposalEndpoint(
    IMediator mediator,
    ILoginUser loginUser
) : Endpoint<DeleteMyProposalReq, ResponseData<bool>>
{
    public override async Task HandleAsync(DeleteMyProposalReq req, CancellationToken ct)
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
/// 删除我的提案请求
/// </summary>
public class DeleteMyProposalReq
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public DeleteMyProposalCommand ToCommand(UserId userId)
    {
        return new DeleteMyProposalCommand
        {
            ProposalId = ProposalId,
            UserId = userId
        };
    }
}