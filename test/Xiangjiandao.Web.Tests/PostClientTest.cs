using Refit;
using Xiangjiandao.Web.Clients;

namespace Xiangjiandao.Web.Tests;

public class PostClientTest
{
    /// <summary>
    /// 测试域名
    /// </summary>
    private const string TestDomain = "https://post.rivtower.cc";

    /// <summary>
    /// Admin 账户 Token
    /// </summary>
    private const string AdminToken = "Basic YWRtaW46ZjRkZjNkNGQyNzlmYTViODA0NjhmY2RmMDM1YzkyZDc=";

    /// <summary>
    /// Refit 客户端实例
    /// </summary>
    private static IPostClient Client => RestService.For<IPostClient>(TestDomain, new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer()
        }
    );

    /// <summary>
    /// 测试给帖子打标签
    /// </summary>
    [Fact]
    public async Task TestLabel()
    {
        var uri = "at://did:plc:vtmfg3vh5xv3v7vrxnphwz4l/app.bsky.feed.post/3lrzshss6bc2t";
        var labelReq = new LabelReq
        {
            Uri = uri,
            Labels = ["blacklist"]
        };

        try
        {
            await Client.Label(labelReq, AdminToken);
        }
        catch (ApiException ex)
        {
            var exContent = ex.Content;
        }

    }

    /// <summary>
    /// 测试查询帖子是否被下架
    /// </summary>
    [Fact]
    public async Task TestIsBanned()
    {
        var userToken = "eyJ0eXAiOiJhdCtqd3QiLCJhbGciOiJIUzI1NiJ9.eyJzY29wZSI6ImNvbS5hdHByb3RvLmFjY2VzcyIsImF1ZCI6ImRpZDp3ZWI6d2ViNS5yaXZ0b3dlci5jYyIsInN1YiI6ImRpZDpwbGM6bjYzdXp2a3BpeXhpNmFsMmwzbGVoNzJvIiwiaWF0IjoxNzUxNTE5OTg2LCJleHAiOjE3NTE1MjcxODZ9.eoWbJe4Zpood5lvq-Gb3rYDPRyo5GQE3vZ6FpBzdCxQ";
        var uri = "at://did:plc:vtmfg3vh5xv3v7vrxnphwz4l/app.bsky.feed.post/3lrzshss6bc2t";
        try
        {
            var isBannedResp = await Client.IsBanned([uri], userToken);
        }
        catch (ApiException ex)
        {
            var exContent = ex.Content;
        }
    }
}