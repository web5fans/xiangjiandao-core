using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminInformation;

/// <summary>
/// 后台公告详情
/// </summary>
[Tags("AdminInformation")]
[HttpPost("/api/v1/admin/information/detail")]
[Authorize(PolicyNames.Admin)]
public class AdminInformationDetailEndpoint(
    InformationQuery query
)
    : Endpoint<AdminInformationDetailReq, ResponseData<AdminInformationDetailVo>>
{
    public override async Task HandleAsync(AdminInformationDetailReq req, CancellationToken ct)
    {
        var result = await query.AdminDetail(id: req.InformationId, ct);
        if (result is null)
        {
            throw new KnownException("公告未找到");
        }

        await SendAsync(
            response: result.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 后台公告详情请求
/// </summary>
public class AdminInformationDetailReq
{
    /// <summary>
    /// 公告 Id
    /// </summary>
    public required InformationId InformationId { get; set; }
}

/// <summary>
/// 后台公告详情响应
/// </summary>
public class AdminInformationDetailVo
{
    /// <summary>
    /// 公告 Id
    /// </summary>
    public required InformationId InformationId { get; set; }

    /// <summary>
    /// 公告标题
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 公告附件 Id
    /// </summary>
    public required string AttachId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public required DateTime CreateAt { get; set; }
}