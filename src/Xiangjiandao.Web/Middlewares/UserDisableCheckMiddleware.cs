using System.IdentityModel.Tokens.Jwt;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Queries;

namespace Xiangjiandao.Web.Middlewares;

/// <summary>
/// 用户禁用检测中间件
/// </summary>
public class UserDisableCheckMiddleware(
    RequestDelegate next
)
{
    public async Task Invoke(HttpContext context)
    {
        var userQuery = context.RequestServices.GetRequiredService<UserQuery>();
        var path = context.Request.Path;

        // 只处理特定前缀的请求
        if (!path.StartsWithSegments("/bsky") &&
            !path.StartsWithSegments("/pds") &&
            !path.StartsWithSegments("/plc") &&
            !path.StartsWithSegments("/post"))
        {
            await next.Invoke(context);
            return;
        }

        // 只处理 jwt 中有 sub 字段的请求
        var handler = new JwtSecurityTokenHandler();
        var auth = context.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(auth))
        {
            await next.Invoke(context);
            return;
        }

        var jwtToken = handler.ReadJwtToken(auth.Replace("Bearer ", string.Empty));
        var userDid = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (userDid is null)
        {
            await next.Invoke(context);
            return;
        }

        var isDisable = await userQuery.UserDisabled(did: userDid, cancellationToken: context.RequestAborted);

        if (isDisable)
        {
            context.Response.StatusCode = 401;
            var response = new ResponseData(false, "该用户已被禁用", 400001);
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        await next.Invoke(context);
    }
}