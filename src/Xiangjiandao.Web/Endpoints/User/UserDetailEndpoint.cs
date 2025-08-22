using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.User;

/// <summary>
/// C端-获取当前登录用户的详细信息
/// </summary>
/// <param name="query"></param>
/// <param name="loginUser"></param>
[Tags("User")]
[HttpPost("/api/v1/user/login-user-detail")]
[Authorize(PolicyNames.Client)]
public class UserDetailEndpoint(
    UserQuery query,
    ILoginUser loginUser) : EndpointWithoutRequest<ResponseData<UserDetailVo>>
{
    public override async Task HandleAsync( CancellationToken ct)
    {
        var user = await query.GetLoginUserById(new UserId(loginUser.Id), ct);
        if (user is null)
        {
            throw new KnownException("用户不存在");
        }

        if (user.Disable)
        {
            throw new KnownException("用户已被禁用",401);
        }
        await SendAsync(user.AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 当前登录用户信息
/// </summary>
public class UserDetailVo
{ 
    /// <summary>
    /// 用户Id
    /// </summary>
    public UserId Id { get; set; } = default!;
    
    /// <summary>
    /// 邮箱
    /// </summary> 
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary> 
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// 手机区号
    /// </summary> 
    public string PhoneRegion { get; set; } = string.Empty;
    
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
    
    /// <summary>
    /// 稻米
    /// </summary> 
    public long Score { get; set; }
    
    /// <summary>
    /// 是否是节点用户
    /// </summary> 
    public bool NodeUser { get; set; } = false;

    /// <summary>
    /// 是否禁用
    /// </summary> 
    public bool Disable { get; set; } = false;
}