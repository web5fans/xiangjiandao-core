using Xiangjiandao.Domain.AggregatesModel.MedalAggregate;

namespace Xiangjiandao.Web.Application.Vo;

public class AdminMedalPageVo
{
    /// <summary>
    /// 勋章Id
    /// </summary>
    public MedalId Id { get; set; } = default!;
    
    /// <summary>
    /// 封面Id
    /// </summary>
    public string AttachId { get; set; } = string.Empty;
    
    /// <summary>
    /// 勋章名称
    /// </summary> 
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获得用户文件 Id
    /// </summary> 
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// 发放用户数量
    /// </summary> 
    public long Quantity { get; set; }
}