using FastEndpoints;
using Microsoft.AspNetCore.Mvc;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminUser;

/// <summary>
/// 后台-邮箱验证码登录
/// </summary>
/// <param name="jwtGenerator"></param>
/// <param name="query"></param>
/// <param name="sMsVerifyCodeUtils"></param>
public class AdminUserLoginWithVerificationCodeEndpoint(
    [FromServices] JwtGenerator jwtGenerator,
    [FromServices] AdminUserQuery query,
    [FromServices] IAliYunSMSVerifyCodeUtils sMsVerifyCodeUtils) : Endpoint<LoginWithVerificationCodeRequest, ResponseData<TokenVo>>
{
    public override void Configure()
    {
        Post("/api/v1/admin/admin-user/login-with-verification-code");
        Description(x=>x.WithTags("AdminUsers"));
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginWithVerificationCodeRequest req,CancellationToken ct)
    {
        
        var user = await query.GetAdminUserByPhone(req.Phone, req.PhoneRegion,ct);
        // 账号不存在
        if (user == null)
        {
            throw new KnownException("管理员账号不存在");
        }
        
        if (user.SecretData == null || user.SecretData == default!)
        {
            throw new KnownException("管理员账号密码不存在");
        }
        
        // 验证密码
        bool verified = PasswordHashGenerator.Verify(req.Password, user.SecretData.Value, user.SecretData.Salt);
        if (!verified)
        {
            throw new KnownException("管理员账号密码可能已被修改");
        }
        
        // 验证邮箱收到的验证码
        var verify = await sMsVerifyCodeUtils.VerifyAsync(req.PhoneRegion, req.Phone, req.Code, CodeType.AdminUserLogin.GetDesc(),ct);
        if (!verify)
        {
            var data = new[]
            {
                new ErrorDataVo()
                {
                    ErrorCode = "VerifyCodeValidator",
                    ErrorMessage = "验证码错误或已过期",
                    PropertyName = "Code"
                }
            };
            throw new KnownException("验证码错误或已过期", 400, data);
        }
        await SendAsync(await jwtGenerator.Generate(
            userData: new AdminUserData()
            {
                Id = user.Id.Id,
                Email = user.Email,
                PhoneRegion = user.PhoneRegion,
                Phone = user.Phone,
                Role = user.Role.ToRoleType()
            }).AsSuccessResponseData(), cancellation: ct);
    }
}


/// <summary>
/// 邮箱验证码登录请求
/// </summary>
public record LoginWithVerificationCodeRequest
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
    
    /// <summary>
    /// 手机收到的验证码
    /// </summary>
    public string Code { get; set; } = string.Empty;
}