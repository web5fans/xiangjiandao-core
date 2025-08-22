namespace Xiangjiandao.Domain.Enums;

/// <summary>
/// 提案状态
/// </summary>
public enum ProposalStatus{

    /// <summary>
    /// 未知
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 审核中
    /// </summary>
    Review = 1,

    /// <summary>
    /// 已通过
    /// </summary>
    Pass = 2,

    /// <summary>
    /// 未通过
    /// </summary>
    Oppose = 3,
}