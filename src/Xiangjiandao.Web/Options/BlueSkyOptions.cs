namespace Xiangjiandao.Web.Options;

/// <summary>
/// BlueSky 配置
/// </summary>
public class BlueSkyOptions
{
    /// <summary>
    /// PDS 域名
    /// </summary>
    public string PdsDomain { get; set; } = string.Empty;

    /// <summary>
    /// BSKY 域名
    /// </summary>
    public string BskyDomain { get; set; } = string.Empty;

    /// <summary>
    /// PLC 域名
    /// </summary>
    public string PlcDomain { get; set; } = string.Empty;

    /// <summary>
    /// Post 域名
    /// </summary>
    public string PostDomain { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱域名
    /// </summary>
    public string EmailDomain { get; set; } = string.Empty;

    /// <summary>
    /// Admin 访问 Token
    /// </summary>
    public string AdminToken { get; set; } = string.Empty;
}