using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xiangjiandao.Web.Application.Queries;

namespace Xiangjiandao.Web.Middlewares;

public class NodeUserCheckMiddleware(
    RequestDelegate next
)
{
    public async Task Invoke(HttpContext context)
    {
        // 日志组件
        var logger = context.RequestServices.GetRequiredService<ILogger<NodeUserCheckMiddleware>>();

        // Post 服务相关客户端
        var userQuery = context.RequestServices.GetRequiredService<UserQuery>();

        // 只拦截发送帖子的接口
        var path = context.Request.Path;
        if (!path.StartsWithSegments("/pds/xrpc/com.atproto.repo.applyWrites"))
        {
            await next.Invoke(context);
            return;
        }

        // 获取 body 中的数据
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
        var userDid = body.SelectToken("$.repo")?.Value<string>() ?? string.Empty;
        logger.LogInformation("SendPost: userDid: {UserDid}", userDid);
        var textList = body.SelectTokens("$.writes[*].value.text");
        var contentList = new List<string>();

        // 如果 Did 为空，跳过
        if (string.IsNullOrEmpty(userDid))
        {
            await next.Invoke(context);
            return;
        }

        contentList.AddRange(textList.Select(item => item.Value<string>() ?? string.Empty));

        logger.LogInformation("SendPost: contentList: {ContentList}", JsonConvert.SerializeObject(contentList));

        var nodeUserTags = new List<string> { "#任务", "#商品" };
        var hasNodeUserTags = contentList.Any(content => nodeUserTags.Any(content.Contains));
        var isNodeUser = await userQuery.IsNodeUser(userDid);
        var requestChecked = !(hasNodeUserTags && !isNodeUser);

        // 通过校验
        if (requestChecked)
        {
            await next.Invoke(context);
            return;
        }

        context.Response.StatusCode = 403;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Forbidden",
            message = "当前用户不是节点用户，帖子内容包含无效的标签"
        });
    }
}