using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Newtonsoft.Json;
using Refit;
using StackExchange.Redis;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Options;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.User;

/// <summary>
/// 用户注册
/// </summary>
[Tags("User")]
[HttpPost("/api/v1/user/register")]
[AllowAnonymous]
public class UserRegisterEndpoint(
    IMediator mediator,
    IBlueSkyClient blueSkyClient,
    UserQuery userQuery,
    IOptions<BlueSkyOptions> blueSkyOptions,
    IConnectionMultiplexer redisClient,
    JwtGenerator jwtGenerator
) : Endpoint<RegisterReq, ResponseData<RegisterTokenVo>>
{
    /// <summary>
    /// 预注册 Redis Key
    /// </summary>
    private const string PreRegisterKey = "xiangjiandao-pre-register";

    public override async Task HandleAsync(RegisterReq req, CancellationToken ct)
    {
        var db = redisClient.GetDatabase();
        var userRegisterInfo = JsonConvert.DeserializeObject<UserRegisterInfo>(db.HashGet(
                key: PreRegisterKey,
                hashField: req.PreRegisterGuid
            ).ToString()
        );

        if (userRegisterInfo is null)
        {
            throw new KnownException("未找到预注册信息");
        }
        
        // 校验用户手机号和邮箱是否已被使用
        var userExists = await userQuery.UserExists(email: userRegisterInfo.Email, phone: userRegisterInfo.Phone, cancellationToken: ct );
        if (userExists)
        {
            throw new KnownException("该手机号或邮箱已被使用");
        }

        // 通过 BlueSky 注册用户，获取 token 和用户 did
        var createBlueSkyAccountReq = new CreateAccountReq
        {
            Email = req.DomainName.Split('.')[0] + "@" + blueSkyOptions.Value.EmailDomain,
            Password = req.Password,
            Handle = req.DomainName
        };

        CreateAccountResp accountInfo;
        try
        {
            accountInfo = await blueSkyClient.CreateAccount(createBlueSkyAccountReq);
        }
        catch (ApiException ex)
        {
            throw new KnownException($"BlueSky 客户端异常, Message: {ex.Message}");
        }

        var blueSkyToken = accountInfo.AccessJwt;
        var did = accountInfo.Did;

        // 完成用户注册
        var command = req.ToCommand(
            did: did,
            email: userRegisterInfo.Email,
            phone: userRegisterInfo.Phone,
            phoneRegion: userRegisterInfo.PhoneRegion
        );

        var userId = await mediator.Send(command, ct);

        // 生成 Token
        var tokenVo = await jwtGenerator.Generate(new UserData
        {
            Id = userId.Id,
            Email = userRegisterInfo.Email,
            Phone = userRegisterInfo.Phone,
            PhoneRegion = userRegisterInfo.PhoneRegion,
            DomainName = req.DomainName,
        });

        await SendAsync(
            response: new RegisterTokenVo
            {
                Token = tokenVo.AccessToken,
                BlueSkyToken = blueSkyToken,
            }.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 用户注册请求
/// </summary>
public class RegisterReq
{
    /// <summary>
    /// 预注册获取 Guid
    /// </summary>
    public required string PreRegisterGuid { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// 用户生日
    /// </summary>
    public DateTimeOffset Birthday { get; set; }

    /// <summary>
    /// 用户域名
    /// </summary>
    public required string DomainName { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public RegisterCommand ToCommand(string did, string email, string phone, string phoneRegion)
    {
        return new RegisterCommand
        {
            DomainName = DomainName,
            Did = did,
            Email = email,
            Phone = phone,
            PhoneRegion = phoneRegion,
        };
    }
}

/// <summary>
/// 用户注册响应
/// </summary>
public class RegisterTokenVo
{
    /// <summary>
    /// 访问非 BlueSky 的接口携带该 Token
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// 访问 BlueSky 的接口使用该 Token
    /// </summary>
    public required string BlueSkyToken { get; set; }
}