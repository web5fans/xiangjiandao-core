using Xiangjiandao.Domain.Enums;
using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;

/// <summary>
/// 投票记录 Id
/// </summary>
public partial record VoteRecordId : IGuidStronglyTypedId;


/// <summary>
/// 投票记录
/// </summary>
public class VoteRecord : Entity<VoteRecordId>
{
    protected VoteRecord()
    {
    }

    /// <summary>
    /// 提案 Id
    /// </summary> 
    public ProposalId ProposalId { get; private set; } = null!;

    /// <summary>
    /// 投票用户 Id
    /// </summary> 
    public UserId UserId { get; private set; } = null!;

    /// <summary>
    /// 投票选择
    /// </summary> 
    public VoteType Choice { get; private set; }
    
    /// <summary>
    /// 从属的提案
    /// </summary>
    public virtual Proposal Proposal { get; private set; } = null!; 

    /// <summary>
    /// 创建时间
    /// </summary> 
    public DateTime CreatedAt { get; private set; } = DateTime.MinValue;

    /// <summary>
    /// 创建者
    /// </summary> 
    public string CreatedBy { get; private set; } = string.Empty;

    /// <summary>
    /// 更新时间
    /// </summary> 
    public UpdateTime UpdatedAt { get; private set; } = new UpdateTime(DateTimeOffset.MinValue);

    /// <summary>
    /// 更新者
    /// </summary> 
    public string UpdatedBy { get; private set; } = string.Empty;

    /// <summary>
    /// 是否删除
    /// </summary> 
    public Deleted Deleted { get; private set; } = new Deleted(false);

    ///<summary>
    /// 创建投票记录
    ///</summary>
    public static VoteRecord Create(
       UserId userId,
       VoteType choose
    )
    {
        var instance = new VoteRecord
        {
           UserId = userId,
           Choice = choose,
        };
        return instance;
    }

    /// <summary>
    /// 软删除投票记录
    /// </summary>
    public bool Delete(){
        Deleted = true;
        return true;
    }
}
