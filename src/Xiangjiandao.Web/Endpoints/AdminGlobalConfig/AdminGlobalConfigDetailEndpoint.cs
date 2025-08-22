using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminGlobalConfig;

/// <summary>
/// 获取全局配置
/// </summary>
[Tags("AdminGlobalConfig")]
[HttpPost("/api/v1/admin/global-config/detail")]
[Authorize(PolicyNames.Admin)]
public class AdminGlobalConfigDetailEndpoint(GlobalConfigQuery query)
    : EndpointWithoutRequest<ResponseData<AdminGlobalConfigDetailVo>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var adminGlobalConfigDetailVo = await query.Detail(ct);
        if (adminGlobalConfigDetailVo is null)
        {
            throw new KnownException("全局配置不存在");
        }

        await SendAsync(
            response: adminGlobalConfigDetailVo.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 全局配置详情
/// </summary>
public class AdminGlobalConfigDetailVo
{
    /// <summary>
    /// 基金规模
    /// </summary> 
    public required long FundScale { get; set; }

    /// <summary>
    /// 发行稻米规模
    /// </summary> 
    public required long IssuePointsScale { get; set; }

    /// <summary>
    /// 基金会公开信息文件
    /// </summary> 
    public required List<string> FoundationPublicDocument { get; set; }

    /// <summary>
    /// 提案通过票数
    /// </summary> 
    public required int ProposalApprovalVotes { get; set; }
}