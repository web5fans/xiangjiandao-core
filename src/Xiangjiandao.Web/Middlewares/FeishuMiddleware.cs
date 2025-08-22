using AspNet.Security.OAuth.Feishu;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Xiangjiandao.Web.Middlewares;

public class FeishuMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/devops"))
        {
            await next(context);
            return;
        }

        var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (result.Succeeded)
        {
            context.User = result.Principal;
            await next(context);
        }
        else
        {
            //飞书认证重定向到飞书登录
            var returnUrl = context.Request.Path + context.Request.QueryString;
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = returnUrl,
                // 添加IsPersistent以保持登录状态
                IsPersistent = true
            };
            await context.ChallengeAsync(
                FeishuAuthenticationDefaults.AuthenticationScheme, authenticationProperties);
        }
    }
}