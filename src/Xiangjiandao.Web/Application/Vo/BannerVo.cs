using Xiangjiandao.Domain.AggregatesModel.BannerAggregate;

namespace Xiangjiandao.Web.Application.Vo;

/// <summary>
/// BannerVo
/// </summary>
public class BannerVo
{
    /// <summary>
    /// BannerId
    /// </summary>
    public BannerId Id { get; set; } = default!;
    
    /// <summary>
    /// Banner 文件 Id
    /// </summary> 
    public string BannerFileId { get;  set; } = string.Empty;

    /// <summary>
    /// 链接地址
    /// </summary> 
    public string LinkAddress { get;  set; } = string.Empty;
}