using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Clients;

namespace Xiangjiandao.Web.Middlewares;

/// <summary>
/// 帖子下架检查中间件
/// </summary>
public class PostTakeOffCheckMiddleware(
    RequestDelegate next
)
{
    public async Task Invoke(HttpContext context)
    {
        // Post 服务相关客户端
        var postClient = context.RequestServices.GetRequiredService<IPostClient>();
        var userQuery = context.RequestServices.GetRequiredService<UserQuery>();
        var logger = context.RequestServices.GetRequiredService<ILogger<PostTakeOffCheckMiddleware>>();

        var path = context.Request.Path;
        if (!path.StartsWithSegments("/pds/xrpc/app.bsky.feed.getPostThread"))
        {
            await next.Invoke(context);
            return;
        }

        // 检查帖子是否被下架
        var postUri = context.Request.Query["uri"].ToString();
        const string domainOrDidPattern = @"at://(?:www\.)?([^/\s]+)";
        var domainOrDid = Regex.Match(postUri, domainOrDidPattern).Groups[1].Value;
        var userDetail = await userQuery.GetUserFromCache(domainOrDid: domainOrDid);
        if (userDetail is null)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "NotFound",
                message = $"Post not found: {postUri}"
            });
            return;
        }

        postUri = postUri.Replace(domainOrDid, userDetail.Did);
        logger.LogInformation("postUri: {PostUri}", postUri);

        var userToken = context.Request.Headers["Authorization"].ToString();
        var isBannedResp = await postClient.IsBanned(uriList: [postUri], authorization: userToken);
        logger.LogInformation("isBannedResp: {IsBannedResp}", JsonConvert.SerializeObject(isBannedResp));

        var isTakeOff = isBannedResp.Data.Any(info => info.IsBanned && info.Uri == postUri);

        if (isTakeOff)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "NotFound",
                message = $"Post not found: {postUri}"
            });
            return;
        }

        await next.Invoke(context);
    }
}