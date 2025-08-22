using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Middlewares;
using Xiangjiandao.Web.Options;

namespace Xiangjiandao.Web.Tests;

/// <summary>
/// 图片内容审核单元测试
/// </summary>
public class ImageModerationMiddlewareTest
{
    /// <summary>
    /// 是否违规
    /// </summary>
    private static bool _invalied = false;

    /// <summary>
    /// HttpServer
    /// </summary>
    private static TestServer Server => new(new WebHostBuilder()
        .ConfigureServices(services =>
            {
                services.AddSingleton(new LoggerFactory().CreateLogger<ImageUploadModerationMiddleware>());

                // Mock 内容审查客户端
                var moderationClientMock = new Mock<IModerationClient>();
                moderationClientMock
                    .Setup(client =>
                        client.ImageModeration(It.IsAny<string>(), It.Is<string>(value => !value.Contains("违规"))))
                    .Returns(new ModerationResult
                    {
                        Pass = true,
                        Reason = string.Empty,
                    });
                moderationClientMock
                    .Setup(client =>
                        client.ImageModeration(It.IsAny<string>(), It.Is<string>(value => value.Contains("违规"))))
                    .Returns(new ModerationResult
                    {
                        Pass = false,
                        Reason = "包含违规内容",
                    });
                services.AddSingleton<IModerationClient>(_ => moderationClientMock.Object);

                // Mock 配置
                var aliYunModerationOptionsMock = new Mock<IOptions<AliYunModerationOptions>>();
                aliYunModerationOptionsMock.Setup(x => x.Value)
                    .Returns(new AliYunModerationOptions
                    {
                        EnableImageModeration = true,
                        ImageCallbackUrl = "callbackUrl" + (_invalied ? "违规" : string.Empty),
                    });
                services.AddSingleton<IOptions<AliYunModerationOptions>>(_ => aliYunModerationOptionsMock.Object);

                // Mock Redis 数据库
                var redisDatabaseMock = new Mock<IDatabase>();
                redisDatabaseMock.Setup(db => db.StringSetAsync(
                        It.IsAny<RedisKey>(),
                        It.IsAny<RedisValue>(),
                        It.IsAny<TimeSpan>(),
                        It.IsAny<bool>(),
                        It.IsAny<When>(),
                        It.IsAny<CommandFlags>())
                    )
                    .ReturnsAsync(true);
                redisDatabaseMock.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                    .ReturnsAsync(true);
                var connectionMultiplexer = new Mock<IConnectionMultiplexer>();
                connectionMultiplexer.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                    .Returns(redisDatabaseMock.Object);
                services.AddSingleton<IConnectionMultiplexer>(_ => connectionMultiplexer.Object);
            }
        )
        .Configure(app =>
        {
            app.UseMiddleware<ImageUploadModerationMiddleware>();
            app.Run(async context =>
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("OK");
            });
        })
    );

    /// <summary>
    /// 正常图片测试
    /// </summary>
    [Fact]
    public async Task NormalImageTest()
    {
        var client = Server.CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Post, "/pds/xrpc/com.atproto.repo.uploadBlob");
        req.Content = new ByteArrayContent([1, 2, 3]);
        req.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
        var response = await client.SendAsync(req);
        Assert.True(response.IsSuccessStatusCode);
    }

    /// <summary>
    /// 异常图片测试
    /// </summary>
    [Fact]
    public async Task ExceptionImageTest()
    {
        _invalied = true;
        var client = Server.CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Post, "/pds/xrpc/com.atproto.repo.uploadBlob");
        req.Content = new ByteArrayContent([1, 2, 3]);
        req.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
        var response = await client.SendAsync(req);
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}