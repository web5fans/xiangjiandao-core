using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminInformation;

/// <summary>
/// 公告列表查询
/// </summary>
[Tags("AdminInformation")]
[HttpPost("/api/v1/admin/information/list")]
[Authorize(PolicyNames.Admin)]
public class AdminInformationListEndpoint(
    InformationQuery query
) : EndpointWithoutRequest<ResponseData<List<AdminInformationListVo>>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await query.AdminList(cancellationToken: ct);
        await SendAsync(
            response: result.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 公告列表查询响应
/// </summary>
public class AdminInformationListVo
{
    /// <summary>
    /// 公告 Id
    /// </summary>
    public required InformationId Id { get; set; }

    /// <summary>
    /// 公告名称
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 附件 Id
    /// </summary>
    public required string AttachId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public required DateTime CreateAt { get; set; }
}