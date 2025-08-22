using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Options;

namespace Xiangjiandao.Web.Middlewares;

/// <summary>
/// 文本审核中间件
/// </summary>
public class TextModerationMiddleware(
    RequestDelegate next
)
{
    public async Task Invoke(HttpContext context)
    {
        // 日志组件
        var logger = context.RequestServices.GetRequiredService<ILogger<TextModerationMiddleware>>();
        // 内容审核客户端
        var moderationClient = context.RequestServices.GetRequiredService<IModerationClient>();
        // 内容审核配置
        var moderationOptions = context.RequestServices.GetRequiredService<IOptions<AliYunModerationOptions>>();

        // 如果没有开启文本审核，则跳过
        if (!moderationOptions.Value.EnableTextModeration)
        {
            await next.Invoke(context);
            return;
        }

        // 只拦截发送帖子的接口
        var path = context.Request.Path;
        if (!path.StartsWithSegments("/pds/xrpc/com.atproto.repo.applyWrites"))
        {
            await next.Invoke(context);
            return;
        }

        logger.LogInformation("处理文本审核");

        // 读取 body 中的数据
        context.Request.EnableBuffering();
        context.Request.Body.Position = 0;
        string bodyJson;
        using (var reader = new StreamReader(
                   context.Request.Body,
                   Encoding.UTF8,
                   detectEncodingFromByteOrderMarks: false,
                   bufferSize: 1024,
                   leaveOpen: true
               ))
        {
            bodyJson = await reader.ReadToEndAsync();
        }

        context.Request.Body.Position = 0;

        var body = JObject.Parse(bodyJson);
        var textList = body.SelectTokens("$.writes[*].value.text");
        var contentList = new List<string>();
        contentList.AddRange(textList.Select(item => item.Value<string>() ?? string.Empty));
        var content = string.Join("\n", contentList);
        logger.LogInformation("文本审核: 内容: {Content}", content);

        ModerationResult result;
        try
        {
            result = moderationClient.TextModeration(TextModerationServices.CommentDetection, content);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Aliyun 文本审核错误");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Internal Server Error",
                message = "服务器内部错误"
            });
            return;
        }

        // 通过校验
        if (result.Pass)
        {
            logger.LogInformation("文本审核通过");
            await next.Invoke(context);
            return;
        }

        logger.LogInformation("帖子内容可能包含违规内容, Reason: {Reason}", result.Reason);
        context.Response.StatusCode = 403;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Forbidden",
            message = "帖子内容可能包含违规内容"
        });
    }
}