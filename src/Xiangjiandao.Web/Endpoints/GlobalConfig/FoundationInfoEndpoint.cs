using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;

namespace Xiangjiandao.Web.Endpoints.GlobalConfig;

/// <summary>
/// 基金会信息查询
/// </summary>
[Tags("GlobalConfig")]
[HttpPost("/api/v1/global-config/foundation-info")]
[AllowAnonymous]
public class FoundationInfoEndpoint(GlobalConfigQuery query) : EndpointWithoutRequest<ResponseData<FoundationInfoVo>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await query.FoundationInfo(cancellationToken: ct);
        if (result is null)
        {
            throw new KnownException("全局配置未找到");
        }

        await SendAsync(
            response: result.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 基金会信息
/// </summary>
public class FoundationInfoVo
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
}