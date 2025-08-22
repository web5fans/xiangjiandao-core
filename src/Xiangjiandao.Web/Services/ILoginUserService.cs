using Xiangjiandao.Web.Shared;

namespace Xiangjiandao.Web.Services;

public interface ILoginUserService
{
    Task<LoginUser> GetLoginUserAsync();
}

public class LoginUserService(
    IHttpContextAccessor httpContextAccessor,
    ILogger<LoginUserService> logger
) : ILoginUserService
{
    /// <summary>
    /// 获取 LoginUser
    /// </summary>
    public Task<LoginUser> GetLoginUserAsync()
    {
        try
        {
            var context = httpContextAccessor.HttpContext;
            if (context == null)
            {
                logger.LogInformation("HttpContext is null");
                return Task.FromResult(new LoginUser(Guid.Empty, string.Empty, string.Empty, false, string.Empty));
            }

            var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ")[^1];

            if (string.IsNullOrEmpty(token))
            {
                token = context.Request.Query["access_token"];
                if (string.IsNullOrEmpty(token))
                {
                    logger.LogInformation("Token is null");
                    return Task.FromResult(new LoginUser(Guid.Empty, string.Empty, string.Empty, false, string.Empty));
                }
            }

            var identity = context.User.Identity;
            if (identity is not { IsAuthenticated: true })
            {
                return Task.FromResult(new LoginUser(Guid.Empty, string.Empty, string.Empty, false, string.Empty));
            }

            var uid = context.User.Claims.First(c => c.Type == "uid").Value;
            // var email = context.User.Claims.First(c => c.Type == "email").Value;
            var type = context.User.Claims.First(c => c.Type == "type").Value;

            return Task.FromResult(new LoginUser(Guid.Parse(uid), string.Empty, token, true, type));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Get loginUser failed");
        }

        return Task.FromResult(new LoginUser(Guid.Empty, string.Empty, string.Empty, false, string.Empty));
    }
}