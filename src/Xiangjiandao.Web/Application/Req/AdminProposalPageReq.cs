using Xiangjiandao.Domain.Enums;

namespace Xiangjiandao.Web.Application.Req;

public record AdminProposalPageReq
{
    /// <summary>
    /// 提案名称
    /// </summary> 
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 发起方名称
    /// </summary> 
    public string InitiatorName { get; set; } = string.Empty;
    
    /// <summary>
    /// 提案状态
    /// </summary> 
    public ProposalStatus Status { get; set; } = ProposalStatus.Unknown;
    
    /// <summary>
    /// 发布开始时间 yyyy-MM-dd
    /// </summary>
    public DateOnly? StartTime { get; set; } 

    /// <summary>
    /// 发布结束时间 yyyy-MM-dd
    /// </summary>
    public DateOnly? EndTime { get; set; } 
    
    /// <summary>
    ///  分页页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// 分页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
}