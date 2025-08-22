namespace Xiangjiandao.Web.Options;

public class AliYunSmsOptions
{
    public string AccessKeyId { get; set; } = string.Empty;
    
    public string AccessKeySecret { get; set; } = string.Empty;
    
    public string SignName  { get; set; } = string.Empty;
    
    public string TemplateCode { get; set; } = string.Empty;
    
    public string EndPoint { get; set; } = string.Empty;
    
    /// <summary>
    /// 短信验证码试错次数
    /// </summary>
    public int Times { get; set; }
    
    
}