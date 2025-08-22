using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetCorePal.Extensions.Jwt;
using Newtonsoft.Json;

namespace Xiangjiandao.Web.Utils;

/// <summary>
/// 登录 Token 
/// </summary>
public class TokenVo
{
    /// <summary>
    /// Jwt token
    /// </summary>
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间
    /// </summary>
    [JsonProperty("expires_in")]
    public string ExpiresIn { get; set; } = string.Empty;

    /// <summary>
    /// 权限范围
    /// </summary>
    [JsonProperty("scope")]
    public string Scope { get; set; } = string.Empty;
}

/// <summary>
/// 用户信息
/// </summary>
public record UserData
{
    /// <summary>
    /// 用户 Id
    /// </summary>
    public required Guid Id { get; set; }

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
    /// 域名
    /// </summary>
    public required string DomainName { get; set; }

}

/// <summary>
/// 管理员信息
/// </summary>
public record AdminUserData
{
    /// <summary>
    /// 管理员 Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 手机号
    /// </summary>
    public required string Phone { get; set; }

    /// <summary>
    /// 手机区号
    /// </summary>
    public required string PhoneRegion { get; set; }
    
    /// <summary>
    /// 角色
    /// </summary>
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Jwt 配置
/// </summary>
public record JwtConfig
{
    /// <summary>
    /// 密钥
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// 验签公钥 Id
    /// </summary>
    public string Kid { get; set; } = string.Empty;

    /// <summary>
    /// 颁发者
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// 受众
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间, 单位分钟
    /// </summary>
    public int ExpirationInMinutes { get; set; }
}

/// <summary>
/// Jwt 生成工具
/// </summary>
public class JwtGenerator(IOptions<JwtConfig> options, IJwtProvider jwtProvider)
{
    /// <summary>
    /// 生成前台用户 Token
    /// </summary>
    public async Task<TokenVo> Generate(UserData userData)
    {
        var jwtConfig = options.Value;
        var claims = new[]
        {
            new Claim("uid", userData.Id.ToString()),
            new Claim("type", PolicyNames.Client),
            new Claim("phone", userData.Phone),
            new Claim("email", userData.Email),
            new Claim("phone-region", userData.PhoneRegion),
            new Claim("domain-name", userData.DomainName),
        };

        var expires = DateTimeOffset.UtcNow.AddMinutes(jwtConfig.ExpirationInMinutes);
        var jwtString = await jwtProvider.GenerateJwtToken(new JwtData(
            jwtConfig.Issuer,
            jwtConfig.Audience,
            claims,
            DateTime.UtcNow,
            expires.UtcDateTime));

        return new TokenVo
        {
            AccessToken = $"Bearer {jwtString}",
            ExpiresIn = expires.ToUnixTimeMilliseconds().ToString(),
            Scope = "Login"
        };
    }

    /// <summary>
    /// 生成管理员 Token
    /// </summary>
    public async Task<TokenVo> Generate(AdminUserData userData)
    {
        var jwtConfig = options.Value;
        var claims = new[]
        {
            new Claim("uid", userData.Id.ToString()),
            new Claim("type", PolicyNames.Admin),
            new Claim("email", userData.Email),
            new Claim("phone", userData.Phone),
            new Claim("phone-region", userData.PhoneRegion),
            new Claim(ClaimTypes.Role, userData.Role),
        };

        var expires = DateTimeOffset.UtcNow.AddMinutes(jwtConfig.ExpirationInMinutes);
        var jwtString = await jwtProvider.GenerateJwtToken(new JwtData(
            jwtConfig.Issuer,
            jwtConfig.Audience,
            claims,
            DateTime.UtcNow,
            expires.UtcDateTime));

        return new TokenVo
        {
            AccessToken = $"Bearer {jwtString}",
            ExpiresIn = expires.ToUnixTimeMilliseconds().ToString(),
            Scope = "Login"
        };
    }

    /// <summary>
    /// 计算 Token
    /// </summary>
#pragma warning disable S1144
    private static string ComputeToken(
#pragma warning restore S1144
        IEnumerable<Claim> claims,
        DateTimeOffset expires,
        byte[] privateKey,
        string issuer,
        string audience,
        string kid
    )
    {
        var rsa = RSA.Create();

        rsa.ImportPkcs8PrivateKey(privateKey, out _);

        var key = new RsaSecurityKey(rsa);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires.UtcDateTime,
            signingCredentials: credentials
        )
        {
            Header =
            {
                ["kid"] = kid
            }
        };
        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);

        return jwtString;
    }
}

/// <summary>
/// Token 类型
/// </summary>
public abstract class PolicyNames
{
    /// <summary>
    /// 后台 Token
    /// </summary>
    public const string Admin = "admin";
    
    /// <summary>
    /// 管理员
    /// </summary>
    public const string AdminOnly = "adminOnly";

    /// <summary>
    /// C 端
    /// </summary>
    public const string Client = "client";
}


/// <summary>
/// Token 类型
/// </summary>
public abstract class TokenType
{
    /// <summary>
    /// 后台 Token
    /// </summary>
    public const string Admin = "admin";

    /// <summary>
    /// C 端
    /// </summary>
    public const string Client = "client";
}