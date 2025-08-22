using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.DomainEvents;

namespace Xiangjiandao.Domain.AggregatesModel.ScoreDistributeRecordAggregate;

/// <summary>
/// 后台稻米发放记录 Id
/// </summary>
public partial record ScoreDistributeRecordId : IGuidStronglyTypedId;

/// <summary>
/// 后台稻米发放记录
/// </summary>
public class ScoreDistributeRecord : Entity<ScoreDistributeRecordId>, IAggregateRoot
{
    protected ScoreDistributeRecord()
    {
    }

    /// <summary>
    /// 用户Id
    /// </summary>
    public UserId UserId { get; private set; } = default!;
    
    /// <summary>
    /// 用户昵称
    /// </summary> 
    public string NickName { get; private set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary> 
    public string Phone { get; private set; } = string.Empty;
    
    /// <summary>
    /// 用户手机区号
    /// </summary> 
    public string PhoneRegion { get; private set; } = string.Empty;
    
    /// <summary>
    /// 用户邮箱
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// 发放稻米
    /// </summary> 
    public long Score { get; private set; }

    /// <summary>
    /// 获取时间
    /// </summary>
    public DateTimeOffset GetTime { get; private set; } = DateTimeOffset.MinValue;

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
    /// 创建后台稻米发放记录
    ///</summary>
    public static ScoreDistributeRecord Create(
        UserId userId,
        string nickName,
        string phone,
        string phoneRegion,
        string email,
        long score,
        string createdBy
    )
    {
        var instance = new ScoreDistributeRecord
        {
            UserId = userId,
            NickName = nickName,
            Phone = phone,
            PhoneRegion = phoneRegion,
            Email = email,
            Score = score,
            GetTime = DateTimeOffset.Now,
            CreatedAt = DateTimeOffset.Now,
            CreatedBy = createdBy
        };
        // 创建用户稻米明细记录，user增加稻米
        instance.AddDomainEvent(new ScoreDistributeRecordCreatedDomainEvent(instance));
        return instance;
    }

    /// <summary>
    /// 软删除后台稻米发放记录
    /// </summary>
    public bool Delete()
    {
        Deleted = true;
        return true;
    }
}