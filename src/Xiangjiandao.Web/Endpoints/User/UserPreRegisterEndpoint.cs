using System.Text.RegularExpressions;
using FastEndpoints;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Newtonsoft.Json;
using StackExchange.Redis;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.User;

/// <summary>
/// 用户预注册, 返回一个用户 Id, 使用该 id 完成后续注册流程
/// </summary>
[Tags("User")]
[HttpPost("/api/v1/user/pre-register")]
[AllowAnonymous]
public class UserPreRegisterEndpoint(
    IMediator mediator,
    IAliYunSMSVerifyCodeUtils aliYunSmsVerifyCodeUtils,
    IConnectionMultiplexer redisClient,
    IEmailVerifyCodeUtils emailVerifyCodeUtils
) : Endpoint<PreRegisterReq, ResponseData<string>>
{
    /// <summary>
    /// 预注册 Redis Key
    /// </summary>
    private const string PreRegisterKey = "xiangjiandao-pre-register";

    public override async Task HandleAsync(PreRegisterReq req, CancellationToken ct)
    {
        switch (req.RegisterType)
        {
            // 验证手机或邮箱验证码
            case RegisterType.Email:
            {
                var isVerified = await emailVerifyCodeUtils.VerifyAsync(
                    emailAddress: req.Email,
                    code: req.VerifyCode,
                    codeType: CodeType.Register
                );
                if (!isVerified)
                {
                    throw new KnownException("非法的邮箱验证码");
                }

                break;
            }
            case RegisterType.Phone:
            {
                var isVerified = await aliYunSmsVerifyCodeUtils.VerifyAsync(
                    phoneRegion: req.PhoneRegion,
                    phone: req.Phone,
                    code: req.VerifyCode,
                    scene: CodeType.Register.GetDesc(),
                    cancellationToken: ct
                );
                if (!isVerified)
                {
                    throw new KnownException("非法的手机验证码");
                }

                break;
            }
            case RegisterType.Unknown:
            default:
                throw new KnownException("非法的预注册类型");
        }

        var preRegisterGuid = Guid.NewGuid().ToString();
        var db = redisClient.GetDatabase();

        db.HashSet(
            key: PreRegisterKey,
            hashField: preRegisterGuid,
            value: JsonConvert.SerializeObject(new UserRegisterInfo
            {
                Email = req.Email,
                Phone = req.Phone,
                PhoneRegion = req.PhoneRegion,
            })
        );
        db.KeyExpire(key: PreRegisterKey, TimeSpan.FromMinutes(30));

        await SendAsync(
            response: preRegisterGuid.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 用户注册信息
/// </summary>
public class UserRegisterInfo
{
    /// <summary>
    /// 邮箱
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public required string Phone { get; set; }

    /// <summary>
    /// 手机区号
    /// </summary>
    public required string PhoneRegion { get; set; }
}

/// <summary>
/// 用户预注册请求
/// </summary>
public class PreRegisterReq
{
    /// <summary>
    /// 注册类型 1-邮箱 2-手机号
    /// </summary>
    public required RegisterType RegisterType { get; set; }

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
    public required string VerifyCode { get; set; } = string.Empty;
}

public enum RegisterType
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

/// <summary>
/// 请求校验器
/// </summary>
public class PreRegisterReqValidator : AbstractValidator<PreRegisterReq>
{
    /// <summary>
    /// 邮箱正则表达式
    /// </summary>
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    /// <summary>
    /// 手机号正则表达式
    /// </summary>
    private static readonly Regex PhoneRegex = new Regex(@"^1[3-9]\d{9}$", RegexOptions.Compiled);

    public PreRegisterReqValidator()
    {
        RuleFor(req => req)
            .Must(req => !string.IsNullOrEmpty(req.Email) || !string.IsNullOrEmpty(req.Phone))
            .WithMessage("邮箱和手机号不能同时为空")
            .When(req => !string.IsNullOrEmpty(req.Email))
            .Must(req => EmailRegex.IsMatch(req.Email))
            .WithMessage("邮箱格式不合法")
            .When(req => !string.IsNullOrEmpty(req.Phone))
            .Must(req => PhoneRegex.IsMatch(req.Phone))
            .WithMessage("手机号不合法");
    }
}