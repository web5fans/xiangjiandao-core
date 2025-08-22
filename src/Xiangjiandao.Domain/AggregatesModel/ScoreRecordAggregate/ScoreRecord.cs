using Xiangjiandao.Domain.Enums;
using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.DomainEvents;

namespace Xiangjiandao.Domain.AggregatesModel.ScoreRecordAggregate;

/// <summary>
/// 稻米记录 Id
/// </summary>
public partial record ScoreRecordId : IGuidStronglyTypedId;


/// <summary>
/// 稻米记录
/// </summary>
public class ScoreRecord : Entity<ScoreRecordId>, IAggregateRoot
{
    protected ScoreRecord()
    {
    }

    /// <summary>
    /// 所属用户
    /// </summary> 
    public UserId UserId { get; private set; } = null!;

    /// <summary>
    /// 稻米来源类型
    /// </summary> 
    public ScoreSourceType Type { get; private set; } = ScoreSourceType.Unknown;

    /// <summary>
    /// 获得原因
    /// </summary> 
    public string Reason { get; private set; } = string.Empty;

    /// <summary>
    /// 稻米数量
    /// </summary> 
    public long Score { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary> 
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.MinValue;

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
    /// 创建稻米记录
    ///</summary>
    public static ScoreRecord Create(
       UserId userId,
       ScoreSourceType type,
       string reason,
       long score
    )
    {
        var instance = new ScoreRecord
        {
           UserId = userId,
           Type = type,
           Reason = reason,
           Score = score,
           CreatedAt = DateTimeOffset.Now,
        };
        instance.AddDomainEvent(new ScoreRecordCreatedDomainEvent(instance));
        return instance;
    }

    /// <summary>
    /// 软删除稻米记录
    /// </summary>
    public bool Delete(){
        Deleted = true;
        return true;
    }
}
