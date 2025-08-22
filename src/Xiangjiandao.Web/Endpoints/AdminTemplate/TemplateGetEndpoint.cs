using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Options;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminTemplate;

/// <summary>
/// 模板Id配置查询
/// </summary>
[Tags("Templates")]
[HttpPost("/api/v1/admin/template/get")]
[Authorize(PolicyNames.AdminOnly)]
public class TemplateGetEndpoint(
    IOptions<TemplateOption> options): EndpointWithoutRequest<ResponseData<AdminTemplateIdVo>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result =  new AdminTemplateIdVo
        {
            MedalDistributionTemplateId = options.Value.MedalDistribution,
            ScoreDistributionTemplateId = options.Value.ScoreDistribution
        };
        await SendAsync( result.AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 模板Id配置信息
/// </summary>
public class AdminTemplateIdVo
{
    /// <summary>
    /// 勋章发放
    /// </summary>
    public string MedalDistributionTemplateId { get; set; } = string.Empty;
    
    /// <summary>
    /// 稻米发放
    /// </summary>
    public string ScoreDistributionTemplateId { get; set; } = string.Empty;
}