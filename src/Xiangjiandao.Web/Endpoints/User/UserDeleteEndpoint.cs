using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Options;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.User;

/// <summary>
/// 删除用户
/// </summary>
[Tags("User")]
[HttpPost("/api/v1/user/delete")]
[AllowAnonymous]
public class UserDeleteEndpoint(
    IAliYunSMSVerifyCodeUtils aliYunSmsVerifyCodeUtils,
    IEmailVerifyCodeUtils emailVerifyCodeUtils,
    IBlueSkyClient blueSkyClient,
    IMediator mediator,
    IOptions<BlueSkyOptions> blueSkyOptions,
    UserQuery userQuery
) : Endpoint<UserDeleteReq, ResponseData<bool>>
{
    public override async Task HandleAsync(UserDeleteReq req, CancellationToken ct)
    {
        switch (req.UserDeleteType)
        {
            // 验证手机或邮箱验证码
            case UserDeleteType.Email:
            {
                var isVerified = await emailVerifyCodeUtils.VerifyAsync(
                    emailAddress: req.Email,
                    code: req.VerifyCode,
                    codeType: CodeType.DeleteAccount
                );
                if (!isVerified)
                {
                    throw new KnownException("非法的邮箱验证码");
                }

                break;
            }
            case UserDeleteType.Phone:
            {
                var isVerified = await aliYunSmsVerifyCodeUtils.VerifyAsync(
                    phoneRegion: req.PhoneRegion,
                    phone: req.Phone,
                    code: req.VerifyCode,
                    scene: CodeType.DeleteAccount.GetDesc(),
                    cancellationToken: ct
                );
                if (!isVerified)
                {
                    throw new KnownException("非法的手机验证码");
                }

                break;
            }
            case UserDeleteType.Unknown:
            default:
                throw new KnownException("非法的验证类型");
        }

        var userDeleteInfo = await userQuery.GetUserDeleteInfo(req.Phone, req.Email, ct);
        if (userDeleteInfo is null)
        {
            throw new KnownException("用户未找到");
        }

        var userDeleteReq = new DeleteAccountReq
        {
            Did = userDeleteInfo.Did,
        };
        await blueSkyClient.DeleteAccount(userDeleteReq, blueSkyOptions.Value.AdminToken);

        var command = req.ToCommand();
        
        await mediator.Send(command, ct);

        await SendAsync(
            response: true.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 重置密码信息
/// </summary>
public class UserDeleteInfo
{
    /// <summary>
    /// 用户 Did
    /// </summary>
    public required string Did { get; set; }
}

/// <summary>
/// 重置用户密码请求
/// </summary>
public class UserDeleteReq
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
    /// 登录类型 1-邮箱，2-手机号
    /// </summary>
    public required UserDeleteType UserDeleteType { get; set; } = 0;

    /// <summary>
    /// 请求转命令
    /// </summary>
    public UserDeleteCommand ToCommand()
    {
        return new UserDeleteCommand
        {
            Phone = Phone,
            Email = Email,
        };
    }
}

/// <summary>
/// 删除账户验证类型
/// </summary>
public enum UserDeleteType
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