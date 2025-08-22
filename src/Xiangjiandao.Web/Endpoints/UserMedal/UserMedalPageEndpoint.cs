using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.MedalAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;

namespace Xiangjiandao.Web.Endpoints.UserMedal;

/// <summary>
/// 用户勋章
/// </summary>
[Tags("UserMedals")]
[HttpPost("/api/v1/user-medal/page")]
[AllowAnonymous]
public class UserMedalPageEndpoint(UserMedalQuery query,
    UserQuery userQuery)
    : Endpoint<UserMedalPageReq, ResponseData<PagedData<UserMedalPageVo>>>
{
    public override async Task HandleAsync(UserMedalPageReq req, CancellationToken ct)
    {
        var user = await userQuery.GetUserByDomainName(req.DomainName, ct);
        if (user is null)
        {
            throw new KnownException("用户不存在");
        }
        var result = await query.Page(user.Id, req, ct);
        await SendAsync(result.AsSuccessResponseData(), cancellation: ct);
    }
}

public record UserMedalPageReq
{
    /// <summary>
    /// 用户域名
    /// </summary>
    public string DomainName { get; set; } = string.Empty;
    
    /// <summary>
    /// 页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// 分页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
}

public class UserMedalPageVo
{
    /// <summary>
    /// 勋章 Id
    /// </summary> 
    public MedalId MedalId { get; set; } = null!;
    
    /// <summary>
    /// 封面 Id
    /// </summary> 
    public string AttachId { get; set; } = string.Empty;

    /// <summary>
    /// 勋章名称
    /// </summary> 
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获得时间
    /// </summary> 
    public DateTimeOffset? GetTime { get; set; } 
}