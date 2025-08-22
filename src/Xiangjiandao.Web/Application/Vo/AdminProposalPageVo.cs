using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.Enums;

namespace Xiangjiandao.Web.Application.Vo;

public class AdminProposalPageVo
{
    public ProposalId Id { get; set; } = default!;
    
    /// <summary>
    /// 提案名称
    /// </summary> 
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 发起方头像
    /// </summary> 
    public string InitiatorAvatar { get; set; } = string.Empty;
    
    /// <summary>
    /// 发起方名称
    /// </summary> 
    public string InitiatorName { get; set; } = string.Empty;
    
    /// <summary>
    /// 提案状态
    /// </summary> 
    public ProposalStatus Status { get; set; } = ProposalStatus.Unknown;
    
    /// <summary>
    /// 创建时间
    /// </summary> 
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.MinValue;
    
    /// <summary>
    /// 投票截至时间
    /// </summary> 
    public DateTimeOffset EndAt { get; set; } = DateTimeOffset.MinValue;
    
    /// <summary>
    /// 是否上架
    /// </summary>
    public bool OnShelf { get; set; }
}