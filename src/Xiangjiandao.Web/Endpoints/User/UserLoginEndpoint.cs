using System.Net;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Refit;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.User;

/// <summary>
/// 用户登录
/// </summary>
[Tags("User")]
[HttpPost("/api/v1/user/login")]
[AllowAnonymous]
public class UserLoginEndpoint(
    JwtGenerator jwtGenerator,
    IBlueSkyClient blueSkyClient,
    UserQuery userQuery
) : Endpoint<UserLoginReq, ResponseData<LoginTokenVo>>
{
    public override async Task HandleAsync(UserLoginReq req, CancellationToken ct)
    {
        var userLoginInfo = await userQuery.UserLoginInfo(
            loginType: req.LoginType,
            domainName: req.DomainName,
            email: req.Email,
            phone: req.Phone,
            cancellationToken: ct
        );


        if (userLoginInfo is null)
        {
            throw new KnownException("未找到该用户");
        }

        if (userLoginInfo.Disable)
        {
            throw new KnownException("该用户已被禁用");
        }

        // 获取 BlueSky Token
        var createSessionReq = new CreateSessionReq
        {
            Identifier = userLoginInfo.DomainName,
            Password = req.Password,
            AuthFactorToken = string.Empty,
            AllowTakendown = true
        };

        CreateSessionResp session;
        try
        {
            session = await blueSkyClient.CreateSession(createSessionReq);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new KnownException("账户或密码错误");
            }

            throw new KnownException($"BlueSky 客户端错误， Message: {ex.Message}");
        }

        var blueSkyToken = session.AccessJwt;
        var blueSkyRefreshToken = session.RefreshJwt;

        var tokenVo = await jwtGenerator.Generate(new UserData
        {
            Id = userLoginInfo.UserId.Id,
            Email = userLoginInfo.Email,
            Phone = userLoginInfo.Phone,
            PhoneRegion = userLoginInfo.PhoneRegion,
            DomainName = userLoginInfo.DomainName,
        });

        await SendAsync(
            response: new LoginTokenVo
            {
                Token = tokenVo.AccessToken,
                BlueSkyToken = blueSkyToken,
                BlueSkyRefreshToken = blueSkyRefreshToken,
            }.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 用户登录信息
/// </summary>
public class UserLoginInfo
{
    /// <summary>
    /// 用户域名
    /// </summary>
    public required string DomainName { get; set; }

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

    /// <summary>
    /// 用户 Id
    /// </summary>
    public required UserId UserId { get; set; }

    /// <summary>
    /// 是否被禁用
    /// </summary>
    public required bool Disable { get; set; }
}

/// <summary>
/// 用户登录响应
/// </summary>
public class LoginTokenVo
{
    /// <summary>
    /// 访问非 BlueSky 的接口携带该 Token
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// 访问 BlueSky 的接口使用该 Token
    /// </summary>
    public required string BlueSkyToken { get; set; }

    /// <summary>
    /// BlueSky RefreshToken
    /// </summary>
    public required string BlueSkyRefreshToken { get; set; }
}

/// <summary>
/// 用户登录请求
/// </summary>
public class UserLoginReq
{
    /// <summary>
    /// 用户域名
    /// </summary>
    public string DomainName { get; set; } = string.Empty;

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
    public string PhoneRegion { get; set; } = "86";

    /// <summary>
    /// 登录密码
    /// </summary>
    public required string Password { get; set; } = string.Empty;

    /// <summary>
    /// 登录类型 1-域名，2-邮箱，3-手机号
    /// </summary>
    public required LoginType LoginType { get; set; }
}

/// <summary>
/// 请求验证器
/// </summary>
public class UserLoginReqValidator : AbstractValidator<UserLoginReq>
{
    public UserLoginReqValidator()
    {
        RuleFor(req => req.Email)
            .NotEmpty()
            .When(req => req.LoginType == LoginType.Email)
            .WithMessage("使用邮箱登录时，邮箱不能为空");
        RuleFor(req => req.Phone)
            .NotEmpty()
            .When(req => req.LoginType == LoginType.Phone)
            .WithMessage("使用手机号登录时，手机号不能为空");
        RuleFor(req => req.DomainName)
            .NotEmpty()
            .When(req => req.LoginType == LoginType.DomainName)
            .WithMessage("使用域名登录时，域名不能为空");
    }
}

/// <summary>
/// 登录类型
/// </summary>
public enum LoginType
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 域名
    /// </summary>
    DomainName = 1,

    /// <summary>
    /// 邮箱
    /// </summary>
    Email = 2,

    /// <summary>
    /// 手机号
    /// </summary>
    Phone = 3,
}