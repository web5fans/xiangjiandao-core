using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Options;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.User;

/// <summary>
/// 重置用户密码
/// </summary>
[Tags("User")]
[HttpPost("/api/v1/user/reset-password")]
[AllowAnonymous]
public class ResetPasswordEndpoint(
    IAliYunSMSVerifyCodeUtils aliYunSmsVerifyCodeUtils,
    IEmailVerifyCodeUtils emailVerifyCodeUtils,
    IBlueSkyClient blueSkyClient,
    IOptions<BlueSkyOptions> blueSkyOptions,
    UserQuery userQuery
) : Endpoint<ResetPasswordReq, ResponseData<bool>>
{
    public override async Task HandleAsync(ResetPasswordReq req, CancellationToken ct)
    {
        switch (req.ResetPasswordType)
        {
            // 验证手机或邮箱验证码
            case ResetPasswordType.Email:
            {
                var isVerified = await emailVerifyCodeUtils.VerifyAsync(
                    emailAddress: req.Email,
                    code: req.VerifyCode,
                    codeType: CodeType.ResetPassword
                );
                if (!isVerified)
                {
                    throw new KnownException("非法的邮箱验证码");
                }

                break;
            }
            case ResetPasswordType.Phone:
            {
                var isVerified = await aliYunSmsVerifyCodeUtils.VerifyAsync(
                    phoneRegion: req.PhoneRegion,
                    phone: req.Phone,
                    code: req.VerifyCode,
                    scene: CodeType.ResetPassword.GetDesc(),
                    cancellationToken: ct
                );
                if (!isVerified)
                {
                    throw new KnownException("非法的手机验证码");
                }

                break;
            }
            case ResetPasswordType.Unknown:
            default:
                throw new KnownException("非法的验证类型");
        }

        var resetPasswordInfo = await userQuery.GetResetPasswordInfo(phone: req.Phone, email: req.Email, ct);
        if (resetPasswordInfo is null)
        {
            throw new KnownException("用户未找到");
        }

        var resetPasswordReq = new UpdateAccountPasswordReq
        {
            Did = resetPasswordInfo.Did,
            Password = req.Password,
        };
        await blueSkyClient.UpdateAccountPassword(resetPasswordReq, blueSkyOptions.Value.AdminToken);

        await SendAsync(
            response: true.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 重置密码信息
/// </summary>
public class ResetPasswordInfo
{
    /// <summary>
    /// 用户 Did
    /// </summary>
    public required string Did { get; set; }
}

/// <summary>
/// 重置用户密码请求
/// </summary>
public class ResetPasswordReq
{
    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 如果是手机号，需要传区号
    /// </summary>
    public string PhoneRegion { get; set; } = "86";

    /// <summary>
    /// 验证码
    /// </summary>
    public required string VerifyCode { get; set; }

    /// <summary>
    /// 重置的密码
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// 登录类型 1-邮箱，2-手机号
    /// </summary>
    public required ResetPasswordType ResetPasswordType { get; set; } = 0;
}

/// <summary>
/// 重置密码验证类型类型
/// </summary>
public enum ResetPasswordType
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 邮箱
    /// </summary>
    Email = 1,

    /// <summary>
    /// 手机号
    /// </summary>
    Phone = 2,
}