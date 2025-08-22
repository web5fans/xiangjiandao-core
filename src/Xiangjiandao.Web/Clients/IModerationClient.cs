using AlibabaCloud.SDK.Green20220302;
using AlibabaCloud.SDK.Green20220302.Models;
using AlibabaCloud.TeaUtil;
using AlibabaCloud.TeaUtil.Models;
using NetCorePal.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xiangjiandao.Web.Clients;

/// <summary>
/// 内容审查客户端
/// </summary>
public interface IModerationClient
{
    /// <summary>
    /// 文本审核
    /// </summary>
    ModerationResult TextModeration(string service, string content);

    /// <summary>
    /// 图片检测
    /// </summary>
    ModerationResult ImageModeration(string service, string imageUrl);
}

/// <summary>
/// 审核结果
/// </summary>
public class ModerationResult
{
    /// <summary>
    /// 是否通过审核
    /// </summary>
    public required bool Pass { get; set; }

    /// <summary>
    /// 未通过原因
    /// </summary>
    public required string Reason { get; set; }
}

/// <summary>
/// 内容审核客户端实现
/// </summary>
public class ModerationClient(
    Client client,
    ILogger<ModerationClient> logger
) : IModerationClient
{
    /// <summary>
    /// 文本审核
    /// </summary>
    public ModerationResult TextModeration(string service, string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return new ModerationResult
            {
                Pass = true,
                Reason = string.Empty,
            };
        }

        var task = new Dictionary<string, object>
        {
            { "content", content }
        };

        var req = new TextModerationRequest
        {
            Service = service,
            ServiceParameters = JsonConvert.SerializeObject(task),
        };
        var runtime = new RuntimeOptions
        {
            ReadTimeout = 10000,
            ConnectTimeout = 10000,
        };

        TextModerationResponse response;
        try
        {
            response = client.TextModerationWithOptions(req, runtime);
            if (response.Body is null ||
                Common.EqualNumber(500, Common.AssertAsNumber(response.StatusCode)) ||
                Common.EqualString("500", Convert.ToString(response.Body.Code)))
            {
                logger.LogInformation("AliYun 内容审核服务错误, Message: {Message}", response.Body?.Message ?? string.Empty);
                throw new KnownException("AliYun 内容审核服务错误");
            }
        }
        catch (Exception e)
        {
            logger.LogInformation(e, "AliYun 内容审核服务错误");
            throw new KnownException("AliYun 内容审核服务错误");
        }

        var riskLever = string.IsNullOrEmpty(response.Body.Data.Reason)
            ? string.Empty
            : JObject.Parse(response.Body.Data.Reason).SelectToken("$.riskLevel")?.Value<string>() ??
              string.Empty;
        return new ModerationResult
        {
            Pass = riskLever is "low" or "",
            Reason = response.Body.Data.Reason ?? string.Empty,
        };
    }

    /// <summary>
    /// 图片审核
    /// </summary>
    public ModerationResult ImageModeration(string service, string imageUrl)
    {
        var task = new Dictionary<string, object>
        {
            { "imageUrl", imageUrl },
        };
        var runtime = new RuntimeOptions
        {
            ReadTimeout = 10000,
            ConnectTimeout = 10000,
        };
        var req = new ImageModerationRequest
        {
            Service = service,
            ServiceParameters = JsonConvert.SerializeObject(task),
        };
        ImageModerationResponse response;
        try
        {
            response = client.ImageModerationWithOptions(req, runtime);
            if (response.Body is null ||
                Common.EqualNumber(500, Common.AssertAsNumber(response.StatusCode)) ||
                Common.EqualString("401", Convert.ToString(response.Body.Code)) ||
                Common.EqualString("500", Convert.ToString(response.Body.Code)))
            {
                logger.LogInformation("AliYun 内容审核服务错误, Message: {Message}", response.Body?.Msg ?? string.Empty);
                throw new KnownException("AliYun 内容审核服务错误");
            }
        }
        catch (Exception e)
        {
            logger.LogInformation(e, "AliYun 内容审核服务错误");
            throw new KnownException("AliYun 内容审核服务错误");
        }

        return new ModerationResult
        {
            Pass = response.Body.Data.RiskLevel is "low" or "none" or "",
            Reason = JsonConvert.SerializeObject(response.Body.Data.Result)
        };
    }
}

public static class TextModerationServices
{
    /// <summary>
    /// 用户昵称检测
    /// </summary>
    public static readonly string NickNameDetection = "nickname_detection";

    /// <summary>
    /// 私聊互动内容检测
    /// </summary>
    public static readonly string ChatDetection = "chat_detection";

    /// <summary>
    /// 公聊评论内容检测
    /// </summary>
    public static readonly string CommentDetection = "comment_detection";

    /// <summary>
    /// AIGC 文字检测
    /// </summary>
    public static readonly string AiArtDetection = "ai_art_detection";

    /// <summary>
    /// 广告法合规检测
    /// </summary>
    public static readonly string AdComplianceDetection = "ad_compliance_detection";

    /// <summary>
    /// PGC 教学物料检测
    /// </summary>
    public static readonly string PgcDetection = "pgc_detection";
}

public static class ImageModerationServices
{
    /// <summary>
    /// 通用基线检测
    /// </summary>
    public static readonly string BaselineCheck = "baselineCheck";

    /// <summary>
    /// 头像检测
    /// </summary>
    public static readonly string ProfilePhotoCheck = "profilePhotoCheck";
}