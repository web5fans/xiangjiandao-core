using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;

namespace Xiangjiandao.Web.Endpoints.External;

/// <summary>
/// 拉取图片接口
/// </summary>
[Tags("External")]
[HttpGet("/api/v1/external/image/{ImageId}")]
[AllowAnonymous]
public class ImageFetchEndpoint(
    IConnectionMultiplexer redisClient,
    ILogger<ImageFetchEndpoint> logger
) : Endpoint<FetchImageReq>
{
    public override async Task HandleAsync(FetchImageReq req, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(req.ImageId))
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        logger.LogInformation("Fetch Images: ImageId: {ImageId}", req.ImageId);

        var db = redisClient.GetDatabase();
        // 二进制数据
        var data = await db.StringGetAsync(req.ImageId);
        if (data.IsNullOrEmpty)
        {
            // 图片不存在
            await SendNotFoundAsync(ct);
        }

        // 图片 ContentType
        var contentTypeValue = await db.StringGetAsync("content_type_" + req.ImageId);
        var contentType = contentTypeValue.IsNullOrEmpty ? "application/octet-stream" : contentTypeValue.ToString();

        byte[] imageBytes = data!;
        HttpContext.Response.ContentType = contentType;
        HttpContext.Response.ContentLength = imageBytes.Length;
        await HttpContext.Response.BodyWriter.WriteAsync(imageBytes, ct);
    }
}

/// <summary>
/// 拉取图片请求
/// </summary>
public class FetchImageReq
{
    /// <summary>
    /// 图片 Id
    /// </summary>
    public string ImageId { get; set; } = string.Empty;
}