namespace Xiangjiandao.Web.Options;

/// <summary>
/// 异常日志记录配置
/// </summary>
public class ExceptionlessOptions
{
    /// <summary>
    /// 项目 Api 密钥
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// 服务链接
    /// </summary>
    public string ServerUrl { get; set; } = string.Empty;

    /// <summary>
    /// 是否开启
    /// </summary>
    public bool Enable { get; set; } = false;
}