using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.AggregatesModel.MedalAggregate;

namespace Xiangjiandao.Domain.AggregatesModel.UserAggregate;

/// <summary>
/// 用户勋章 Id
/// </summary>
public partial record UserMedalId : IGuidStronglyTypedId;

/// <summary>
/// 用户勋章
/// </summary>
public class UserMedal : Entity<UserMedalId>, IAggregateRoot
{
    protected UserMedal()
    {
    }

    /// <summary>
    /// 用户 Id
    /// </summary> 
    public UserId UserId { get; private set; } = null!;

    /// <summary>
    /// 勋章 Id
    /// </summary> 
    public MedalId MedalId { get; private set; } = null!;

    /// <summary>
    /// 用户昵称
    /// </summary> 
    public string NickName { get; private set; } = string.Empty;

    /// <summary>
    /// 用户头像
    /// </summary> 
    public string Avatar { get; private set; } = string.Empty;

    /// <summary>
    /// 用户手机号
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
    /// 封面 Id
    /// </summary> 
    public string AttachId { get; private set; } = string.Empty;

    /// <summary>
    /// 勋章名称
    /// </summary> 
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 获得时间
    /// </summary> 
    public DateTimeOffset GetTime { get; private set; } = DateTimeOffset.MinValue;

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
    /// 创建用户勋章
    ///</summary>
    public static UserMedal Create(
        UserId userId,
        MedalId medalId,
        string nickName,
        string avatar,
        string phone,
        string phoneRegion,
        string email,
        string attachId,
        string name,
        DateTimeOffset getTime
    )
    {
        var instance = new UserMedal
        {
            UserId = userId,
            MedalId = medalId,
            NickName = nickName,
            Avatar = avatar,
            Phone = phone,
            PhoneRegion = phoneRegion,
            Email = email,
            AttachId = attachId,
            Name = name,
            GetTime = getTime,
        };
        return instance;
    }

    /// <summary>
    /// 软删除用户勋章
    /// </summary>
    public bool Delete()
    {
        Deleted = true;
        return true;
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    public void ModifyUserInfo(
        string nickName,
        string avatar,
        string phone,
        string phoneRegion
    )
    {
        NickName = nickName;
        Avatar = avatar;
        Phone = phone;
        PhoneRegion = phoneRegion;
    }
}