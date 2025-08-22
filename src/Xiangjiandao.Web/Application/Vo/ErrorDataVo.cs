namespace Xiangjiandao.Web.Application.Vo;

public record ErrorDataVo
{
    public string ErrorCode { get; set; } = string.Empty;
    
    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// 属性名
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;
}