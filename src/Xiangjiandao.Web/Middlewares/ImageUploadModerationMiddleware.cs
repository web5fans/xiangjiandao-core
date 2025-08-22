using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Options;

namespace Xiangjiandao.Web.Middlewares;

/// <summary>
/// 图片上传审核中间件
/// </summary>
public class ImageUploadModerationMiddleware(
    RequestDelegate next
)
{
    public async Task Invoke(HttpContext context)
    {
        // 日志组件
        var logger = context.RequestServices.GetRequiredService<ILogger<ImageUploadModerationMiddleware>>();
        // 用于临时存储图片
        var redisClient = context.RequestServices.GetRequiredService<IConnectionMultiplexer>();
        // 内容审核客户端
        var moderationClient = context.RequestServices.GetRequiredService<IModerationClient>();
        // 内容审核配置
        var moderationOptions = context.RequestServices.GetRequiredService<IOptions<AliYunModerationOptions>>();

        // 如果没有开启图片审核，则跳过
        if (!moderationOptions.Value.EnableImageModeration)
        {
            await next.Invoke(context);
            return;
        }

        // 只拦截上传接口
        var path = context.Request.Path;
        if (!path.StartsWithSegments("/pds/xrpc/com.atproto.repo.uploadBlob"))
        {
            await next.Invoke(context);
            return;
        }

        // 只拦截图片
        var contentType = context.Request.ContentType ?? string.Empty;
        if (!contentType.StartsWith("image/"))
        {
            await next.Invoke(context);
            return;
        }

        logger.LogInformation("处理图片审核");

        // 获取原始图片流
        context.Request.EnableBuffering();
        context.Request.Body.Position = 0;
        byte[] streamBytes;
        await using (var stream = new MemoryStream())
        {
            await context.Request.Body.CopyToAsync(stream);
            streamBytes = stream.ToArray();
        }

        context.Request.Body.Position = 0;

        // 存入 Redis 等待审查
        var db = redisClient.GetDatabase();
        var moderationId = Guid.NewGuid().ToString();
        await db.StringSetAsync(moderationId, streamBytes, TimeSpan.FromMinutes(10));
        await db.StringSetAsync("content_type_" + moderationId, contentType, TimeSpan.FromMinutes(10));
        logger.LogInformation("图片数据存入 Redis, ModerationId: {ModerationId}", moderationId);

        // Aliyun 图片审查
        ModerationResult result;
        try
        {
            var imageUrl = moderationOptions.Value.ImageCallbackUrl + moderationId;
            result = moderationClient.ImageModeration(ImageModerationServices.BaselineCheck, imageUrl);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Aliyun 图片审核错误");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Internal Server Error",
                message = "服务器内部错误"
            });
            return;
        }
        finally
        {
            // 结束审查释放缓存中的内容
            await db.KeyDeleteAsync(moderationId);
            await db.KeyDeleteAsync("content_type_" + moderationId);
        }

        // 通过校验放行
        if (result.Pass)
        {
            logger.LogInformation("图片审核通过");
            await next.Invoke(context);
            return;
        }

        logger.LogInformation("上传图片可能包含违规内容, Reason: {Reason}", result.Reason);
        context.Response.StatusCode = 403;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Forbidden",
            message = "上传图片可能包含违规内容"
        });
    }
}