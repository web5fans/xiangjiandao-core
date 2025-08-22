using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;

namespace Xiangjiandao.Web.Endpoints.Proposal;

/// <summary>
/// 提案列表
/// </summary>
[Tags("Proposal")]
[HttpPost("/api/v1/proposal/page")]
[AllowAnonymous]
public class ProposalPageEndpoint(
    ProposalQuery query,
    UserQuery userQuery
) : Endpoint<ProposalPageReq, ResponseData<PagedData<ProposalPageVo>>>
{
    public override async Task HandleAsync(ProposalPageReq req, CancellationToken ct)
    {
        var userId = (await userQuery.GetUserByDid(did: req.Did, cancellationToken: ct))?.Id ?? new UserId(Guid.Empty);
        await SendAsync(
            response: await query.Page(
                pageNum: req.PageNum,
                pageSize: req.PageSize,
                status: req.Status,
                userId: userId,
                cancellationToken: ct
            ).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 提案分页响应
/// </summary>
public class ProposalPageVo
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
    /// 发起方 Did
    /// </summary>
    public required string InitiatorDid { get; set; }

    /// <summary>
    /// 发起方域名
    /// </summary>
    public required string InitiatorDomainName { get; set; }

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
    /// 用户选择
    /// </summary>
    public required VoteType Choice { get; set; }
}

/// <summary>
/// 提案分页请求
/// </summary>
public class ProposalPageReq
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// 分页大小
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 提案状态
    /// </summary>
    public ProposalStatus Status { get; set; } = ProposalStatus.Unknown;

    /// <summary>
    /// 查询用户的 Did，当前登陆用户 Did
    /// </summary>
    public string Did { get; set; } = string.Empty;
}