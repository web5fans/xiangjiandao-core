namespace Xiangjiandao.Web.Options;

/// <summary>
/// 阿里云内容审核配置
/// </summary>
public class AliYunModerationOptions
{
    /// <summary>
    /// 是否开启文本审核
    /// </summary>
    public bool EnableTextModeration { get; set; } = false;

    /// <summary>
    /// 是否开启图片审核
    /// </summary>
    public bool EnableImageModeration { get; set; } = false;

    /// <summary>
    /// AccessKeyId
    /// </summary>
    public string AccessKeyId { get; set; } = string.Empty;

    /// <summary>
    /// AccessKeySecret
    /// </summary>
    public string AccessKeySecret { get; set; } = string.Empty;

    /// <summary>
    /// 区域
    /// </summary>
    public string RegionId { get; set; } = string.Empty;

    /// <summary>
    /// Endpoint
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// 获取图片的回调地址
    /// </summary>
    public string ImageCallbackUrl { get; set; } = string.Empty;
}