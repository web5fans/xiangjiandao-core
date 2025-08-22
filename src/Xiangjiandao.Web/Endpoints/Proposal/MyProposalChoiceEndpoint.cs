using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.Proposal;

/// <summary>
/// 我的投票选择
/// </summary>
[Tags("Proposal")]
[HttpPost("/api/v1/proposal/my-proposal-choice")]
[Authorize(PolicyNames.Client)]
public class MyProposalChoiceEndpoint(
    ProposalQuery query,
    ILoginUser loginUser
) : Endpoint<MyProposalChoiceReq, ResponseData<MyProposalChoiceVo>>
{
    public override async Task HandleAsync(MyProposalChoiceReq req, CancellationToken ct)
    {
        var result = await query.MyProposalChoice(
            loginUserId: new UserId(loginUser.Id),
            proposalId: req.ProposalId,
            cancellationToken: ct
        );

        await SendAsync(
            response: result.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 我的投票选择
/// </summary>
public class MyProposalChoiceReq
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }
}

/// <summary>
/// 我的投票选择响应
/// </summary>
public class MyProposalChoiceVo
{
    /// <summary>
    /// 我的投票选择
    /// </summary>
    public required VoteType Choice { get; set; }
}