using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.UserManage;

/// <summary>
/// 根据昵称模糊查找普通用户
/// </summary>
[Tags("UserManages")]
[HttpPost("/api/v1/admin/user-manage/user/search-by-name")]
[Authorize(PolicyNames.Admin)]
public class UserSearchByNameEndpoint(
    UserQuery query): Endpoint<UserSearchByNameReq, ResponseData<List<UserSearchByNameVo>>>
{
    public override async Task HandleAsync(UserSearchByNameReq req, CancellationToken ct)
    {
        
        await SendAsync( await query.UserSearchByName(req, ct).AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 模糊查找全部用户请求
/// </summary>
public record UserSearchByNameReq
{
    /// <summary>
    /// 发布方昵称或者域名
    /// </summary>
    public string NickName { get; set; } = string.Empty;
}

/// <summary>
/// 模糊查找普通用户返回
/// </summary>
public record UserSearchByNameVo
{
    /// <summary>
    /// 用户头像
    /// </summary> 
    public string Avatar { get; set; } = string.Empty;

    /// <summary>
    /// 用户昵称
    /// </summary> 
    public string NickName { get; set; } = string.Empty;
    
    /// <summary>
    /// 完整用户名，域名
    /// </summary> 
    public string DomainName { get; set; } = string.Empty;

    /// <summary>
    /// DID
    /// </summary> 
    public string Did { get; set; } = string.Empty;
}
