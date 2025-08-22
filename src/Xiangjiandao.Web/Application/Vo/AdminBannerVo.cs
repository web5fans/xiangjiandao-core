using Xiangjiandao.Domain.AggregatesModel.BannerAggregate;

namespace Xiangjiandao.Web.Application.Vo;

/// <summary>
/// B端BannerVo
/// </summary>
public class AdminBannerVo
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

    /// <summary>
    /// 排序
    /// </summary> 
    public int Sort { get; set; }
}