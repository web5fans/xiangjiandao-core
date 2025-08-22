using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Green20220302;
using Xiangjiandao.Web.Clients;

namespace Xiangjiandao.Web.Tests;

/// <summary>
/// 阿里云内容审核相关单元测试
/// </summary>
public class AliyunModerationTest
{
    /// <summary>
    /// 修改为正确的 AccessKeyId
    /// </summary>
    private const string AccessKeyId = "";

    /// <summary>
    /// 修改为正确的 AccessKeySecret
    /// </summary>
    private const string AccessKeySecret = "";

    /// <summary>
    /// 修改为正确的 Endpoint
    /// </summary>
    private const string Endpoint = "";

    /// <summary>
    /// Aliyun 内容审查客户端
    /// </summary>
    private static Client Client => new(new Config
    {
        AccessKeyId = AccessKeyId,
        AccessKeySecret = AccessKeySecret,
        Endpoint = Endpoint,
    });

    /// <summary>
    /// 内容审核客户端 Wrapper
    /// </summary>
    private static ModerationClient ModerationClient => new ModerationClient(Client, new LoggerFactory().CreateLogger<ModerationClient>());

    /// <summary>
    /// 文本内容审核测试
    /// </summary>
    [Fact]
    public async Task TextModerationTest()
    {
        var service = TextModerationServices.CommentDetection;
        const string content = "who am i?";
        var result = ModerationClient.TextModeration(service, content);
    }

    /// <summary>
    /// 图像内容审核测试
    /// </summary>
    [Fact]
    public async Task ImageModerationTest()
    {
        var service = ImageModerationServices.BaselineCheck;
        const string imageUrl = "https://bpic.588ku.com/element_origin_min_pic/23/07/11/d32dabe266d10da8b21bd640a2e9b611.jpg!r650";
        var result = ModerationClient.ImageModeration(service, imageUrl);
    }
}