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
/// 我的提案列表
/// </summary>
[Tags("Proposal")]
[HttpPost("/api/v1/proposal/my-proposal-list")]
[Authorize(PolicyNames.Client)]
public class MyProposalListEndpoint(
    ProposalQuery query,
    ILoginUser loginUser
) : Endpoint<MyProposalReq, ResponseData<List<MyProposalVo>>>
{
    public override async Task HandleAsync(MyProposalReq req, CancellationToken ct)
    {
        var userId = new UserId(loginUser.Id);
        await SendAsync(
            response: await query.MyProposalList(
                userId: userId,
                type: req.Type,
                cancellationToken: ct
            ).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 我的提案请求
/// </summary>
public class MyProposalReq
{
    /// <summary>
    /// 类型 0-全部，1-我发布的，2-我参与的
    /// </summary>
    public int Type { get; set; } = 0;
}

/// <summary>
/// 我的提案响应
/// </summary>
public class MyProposalVo
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }

    /// <summary>
    /// 提案名称
    /// </summary> 
    public required string Name { get; set; }

    /// <summary>
    /// 发起方名称
    /// </summary> 
    public required UserId InitiatorId { get; set; }

    /// <summary>
    /// 发起方名称
    /// </summary> 
    public required string InitiatorName { get; set; }

    /// <summary>
    /// 发起方邮箱
    /// </summary> 
    public required string InitiatorEmail { get; set; }

    /// <summary>
    /// 发起方头像
    /// </summary> 
    public required string InitiatorAvatar { get; set; }

    /// <summary>
    /// 反对票数
    /// </summary> 
    public required long OpposeVotes { get; set; }

    /// <summary>
    /// 同意票数
    /// </summary> 
    public required long AgreeVotes { get; set; }

    /// <summary>
    /// 提案状态
    /// </summary> 
    public required ProposalStatus Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary> 
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 发起方 Did
    /// </summary>
    public required string InitiatorDid { get; set; }

    /// <summary>
    /// 发起方域名
    /// </summary>
    public required string InitiatorDomainName { get; set; }

    /// <summary>
    /// 用户选择
    /// </summary>
    public required VoteType Choice { get; set; }
}