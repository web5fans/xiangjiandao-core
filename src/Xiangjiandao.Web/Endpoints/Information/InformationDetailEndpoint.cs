using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;

namespace Xiangjiandao.Web.Endpoints.Information;

/// <summary>
/// 公告详情
/// </summary>
[Tags("Information")]
[HttpPost("/api/v1/information/detail")]
[AllowAnonymous]
public class InformationDetailEndpoint(
    InformationQuery query
)
    : Endpoint<InformationDetailReq, ResponseData<InformationDetailVo>>
{
    public override async Task HandleAsync(InformationDetailReq req, CancellationToken ct)
    {
        var result = await query.Detail(id: req.InformationId, ct);
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
/// 公告详情请求
/// </summary>
public class InformationDetailReq
{
    /// <summary>
    /// 公告 Id
    /// </summary>
    public required InformationId InformationId { get; set; }
}

/// <summary>
/// 公告详情响应
/// </summary>
public class InformationDetailVo
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
