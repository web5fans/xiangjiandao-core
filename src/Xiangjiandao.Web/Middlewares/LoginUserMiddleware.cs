using Xiangjiandao.Web.Services;

namespace Xiangjiandao.Web.Middlewares;

/// <summary>
/// 当前登录用户中间件
/// </summary>
public class LoginUserMiddleware(RequestDelegate next, ILoginUserService service)
{
    internal const string LoginUserKey = "__loginUser";

    public async Task Invoke(HttpContext context)
    {
        context.Items[LoginUserKey] = await service.GetLoginUserAsync();
        await next(context);
    }
}