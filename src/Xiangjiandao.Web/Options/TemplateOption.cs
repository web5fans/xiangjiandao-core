namespace Xiangjiandao.Web.Options;

/// <summary>
/// 模板Id配置
/// </summary>
public class TemplateOption
{
    /// <summary>
    /// 勋章发放
    /// </summary>
    public string MedalDistribution { get; set; } = string.Empty;
    
    /// <summary>
    /// 稻米发放
    /// </summary>
    public string ScoreDistribution { get; set; } = string.Empty;
}