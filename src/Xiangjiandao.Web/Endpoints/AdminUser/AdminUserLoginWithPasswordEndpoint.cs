using FastEndpoints;
using Microsoft.AspNetCore.Mvc;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminUser;

/// <summary>
/// 后台-密码登录-不直接登录
/// </summary>
/// <param name="query"></param>
public class AdminUserLoginWithPasswordEndpoint(
    [FromServices] AdminUserQuery query) : Endpoint<LoginWithPasswordRequest, ResponseData<bool>>
{
    public override void Configure()
    {
        Post("/api/v1/admin/admin-user/login-with-password");
        Description(x=>x.WithTags("AdminUsers"));
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginWithPasswordRequest req,CancellationToken cancellationToken)
    {
        
        var user = await query.GetAdminUserByPhone(req.Phone, req.PhoneRegion, cancellationToken);
        // 账号不存在
        if (user == null)
        {
            var data = new[]
            {
                new ErrorDataVo()
                {
                    ErrorCode = "PhoneValidator",
                    ErrorMessage = "管理员账号不存在",
                    PropertyName = "Phone"
                }
            };
            throw new KnownException("管理员账号不存在", 400, data);
        }
        
        if (user.SecretData == null || user.SecretData == default!)
        {
            var data = new[]
            {
                new ErrorDataVo()
                {
                    ErrorCode = "PhoneValidator",
                    ErrorMessage = "管理员账号密码不存在",
                    PropertyName = "Phone"
                }
            };
            throw new KnownException("管理员账号密码不存在", 400, data);
        }
        // 验证密码
        bool verified = PasswordHashGenerator.Verify(req.Password, user.SecretData.Value, user.SecretData.Salt);
        if (!verified)
        {
            var data = new[]
            {
                new ErrorDataVo()
                {
                    ErrorCode = "PhoneValidator",
                    ErrorMessage = "手机号或密码不正确",
                    PropertyName = "Phone"
                }
            };
            throw new KnownException("手机号或密码不正确", 400, data);
        }
        
        await SendAsync(verified.AsSuccessResponseData(), cancellation: cancellationToken);
    }
}

/// <summary>
/// 密码登录
/// </summary>
public record LoginWithPasswordRequest
{
    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// 手机区号
    /// </summary>
    public string PhoneRegion { get; set; } = "86";

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
}