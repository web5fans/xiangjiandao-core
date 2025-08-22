using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.UserManage;

/// <summary>
/// 模糊查找普通用户
/// </summary>
[Tags("UserManages")]
[HttpPost("/api/v1/admin/user-manage/user/search")]
[Authorize(PolicyNames.Admin)]
public class UserSearchEndpoint(
    UserQuery query): Endpoint<UserSearchReq, ResponseData<List<UserSearchVo>>>
{
    public override async Task HandleAsync(UserSearchReq req, CancellationToken ct)
    {
        
        await SendAsync( await query.UserSearch(req, ct).AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 模糊查找普通用户请求
/// </summary>
public record UserSearchReq
{
    /// <summary>
    /// 手机号或邮箱
    /// </summary>
    public string PhoneOrEmail { get; set; } = string.Empty;
}

/// <summary>
/// 模糊查找普通用户返回
/// </summary>
public record UserSearchVo
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public UserId UserId { get; set; } = null!;
    
    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;
}
