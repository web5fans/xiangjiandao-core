using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using Moq;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Middlewares;
using Xiangjiandao.Web.Options;

namespace Xiangjiandao.Web.Tests;

/// <summary>
/// 文本内容审核单元测试
/// </summary>
public class TextModerationMiddlewareTest
{
    private static TestServer Server => new(new WebHostBuilder()
        .ConfigureServices(services =>
            {
                services.AddSingleton(new LoggerFactory().CreateLogger<TextModerationMiddleware>());

                // Mock 内容审查客户端
                var moderationClientMock = new Mock<IModerationClient>();
                moderationClientMock
                    .Setup(client =>
                        client.TextModeration(It.IsAny<string>(), It.Is<string>(value => !value.Contains("违规"))))
                    .Returns(new ModerationResult
                    {
                        Pass = true,
                        Reason = string.Empty,
                    });
                moderationClientMock
                    .Setup(client =>
                        client.TextModeration(It.IsAny<string>(), It.Is<string>(value => value.Contains("违规"))))
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
                        EnableTextModeration = true,
                    });
                services.AddSingleton<IOptions<AliYunModerationOptions>>(_ => aliYunModerationOptionsMock.Object);
            }
        )
        .Configure(app =>
        {
            app.UseMiddleware<TextModerationMiddleware>();
            app.Run(async context =>
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("OK");
            });
        })
    );

    /// <summary>
    /// 测试正常帖子
    /// </summary>
    [Fact]
    public async Task NormalTest()
    {
        var client = Server.CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Post, "/pds/xrpc/com.atproto.repo.applyWrites");
        req.Content = new StringContent(
            """
            {
              "repo": "did:plc:mxx4bmiloz22yz5tsfpipldh",
              "writes": [
                {
                  "$type": "com.atproto.repo.applyWrites#create",
                  "collection": "app.bsky.feed.post",
                  "rkey": "3lwpukjocbs2l",
                  "value": {
                    "$type": "app.bsky.feed.post",
                    "createdAt": "2025-08-19T02:29:42.987Z",
                    "text": "测试发帖",
                    "langs": [
                      "zh"
                    ]
                  }
                }
              ],
              "validate": true
            }
            """
        );

        var response = await client.SendAsync(req);
        Assert.True(response.IsSuccessStatusCode);
    }

    /// <summary>
    /// 测试异常帖子
    /// </summary>
    [Fact]
    public async Task ExceptoinTest()
    {
        var client = Server.CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Post, "/pds/xrpc/com.atproto.repo.applyWrites");
        req.Content = new StringContent(
            """
            {
              "repo": "did:plc:mxx4bmiloz22yz5tsfpipldh",
              "writes": [
                {
                  "$type": "com.atproto.repo.applyWrites#create",
                  "collection": "app.bsky.feed.post",
                  "rkey": "3lwpukjocbs2l",
                  "value": {
                    "$type": "app.bsky.feed.post",
                    "createdAt": "2025-08-19T02:29:42.987Z",
                    "text": "测试违规发帖",
                    "langs": [
                      "zh"
                    ]
                  }
                }
              ],
              "validate": true
            }
            """
        );

        var response = await client.SendAsync(req);
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}