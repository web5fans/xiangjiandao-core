using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.DomainEvents;

namespace Xiangjiandao.Domain.AggregatesModel.UserAggregate;

/// <summary>
/// 用户 Id
/// </summary>
public partial record UserId : IGuidStronglyTypedId;


/// <summary>
/// 用户
/// </summary>
public class User : Entity<UserId>, IAggregateRoot
{
    protected User()
    {
    }

    /// <summary>
    /// 邮箱
    /// </summary> 
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary> 
    public string Phone { get; private set; } = string.Empty;

    /// <summary>
    /// 手机区号
    /// </summary> 
    public string PhoneRegion { get; private set; } = string.Empty;

    /// <summary>
    /// 用户头像
    /// </summary> 
    public string Avatar { get; private set; } = string.Empty;

    /// <summary>
    /// 用户昵称
    /// </summary> 
    public string NickName { get; private set; } = string.Empty;

    /// <summary>
    /// 简介
    /// </summary> 
    public string Introduction { get; private set; } = string.Empty;

    /// <summary>
    /// 完整用户名，域名
    /// </summary> 
    public string DomainName { get; private set; } = string.Empty;

    /// <summary>
    /// DID
    /// </summary> 
    public string Did { get; private set; } = string.Empty;

    /// <summary>
    /// 稻米
    /// </summary> 
    public long Score { get; private set; }

    /// <summary>
    /// 是否是节点用户
    /// </summary> 
    public bool NodeUser { get; private set; } = false;

    /// <summary>
    /// 是否禁用
    /// </summary> 
    public bool Disable { get; private set; } = false;
    
    /// <summary>
    /// RowVersion
    /// </summary>
    public RowVersion RowVersion { get; private set; } = new RowVersion(0);

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
    /// 用户注册
    ///</summary>
    public static User Register(
       string email,
       string phone,
       string phoneRegion,
       string domainName,
       string did
    )
    {
        var instance = new User
        {
           Email = email,
           Phone = phone,
           PhoneRegion = phoneRegion,
           DomainName = domainName,
           Did = did,
           Avatar = string.Empty,
           NickName = string.Empty,
           Introduction = string.Empty,
           Score = 0,
           NodeUser = false,
        };
        return instance;
    }

    /// <summary>
    /// 软删除用户
    /// </summary>
    public bool Delete()
    {
        DeleteCheck();
        Deleted = true;
        return true;
    }

    /// <summary>
    /// 禁用用户
    /// </summary>
    public void DisableUser()
    {
        DeleteCheck();
        if (Disable)
        {
            throw new KnownException("用户已禁用");
        }
        Disable  = true;
    }
    
    /// <summary>
    /// 启用用户
    /// </summary>
    /// <exception cref="KnownException"></exception>
    public void EnableUser()
    {
        DeleteCheck();
        if (!Disable)
        {
            throw new KnownException("用户已启用");
        }
        Disable = false;
    }
    
    /// <summary>
    /// 设置节点用户
    /// </summary>
    /// <exception cref="KnownException"></exception>
    public void SetNodeUser()
    {
        DeleteCheck();
        if (NodeUser)
        {
            throw new KnownException("用户已经是节点用户");
        }
        NodeUser = true;
    }

    /// <summary>
    /// 取消节点用户
    /// </summary>
    /// <exception cref="KnownException"></exception>
    public void CancelNodeUser()
    {
        DeleteCheck();
        if (!NodeUser)
        {
            throw new KnownException("用户不是节点用户");
        }
        NodeUser = false;
    }

    /// <summary>
    /// 删除检查
    /// </summary>
    /// <exception cref="KnownException"></exception>
    public void DeleteCheck()
    {
        if (Deleted)
        {
            throw new KnownException("用户已删除");
        }
    }

    /// <summary>
    /// 设置BlueSky信息
    /// </summary>
    /// <param name="did"></param>
    /// <exception cref="KnownException"></exception>
    public void SetBlueSkyInfo(string did)
    {
        DeleteCheck();
        if (!string.IsNullOrEmpty(Did))
        {
            throw new KnownException("用户已有did");
        }

        if (!string.IsNullOrEmpty(did))
        {
            Did =  did;
        }
    }

    /// <summary>
    /// 更新稻米
    /// </summary>
    public void UpdateScore(long score)
    {
        if (Score + score < 0)
        {
            throw new KnownException("稻米不足");
        }
        Score += score;
    }

    /// <summary>
    /// 编辑个人资料
    /// </summary>
    /// <param name="avatar"></param>
    /// <param name="nickName"></param>
    /// <param name="introduction"></param>
    public void UpdateProfile(string avatar,  string nickName, string introduction)
    {
        Avatar = avatar;
        NickName = nickName;
        Introduction = introduction;
        AddDomainEvent(new UserModifiedDomainEvent(this));
    }
    
    /// <summary>
    /// 修改邮箱
    /// </summary>
    /// <param name="email"></param>
    public void UpdateEmail(string email)
    {
        Email = email;
        AddDomainEvent(new UserModifiedDomainEvent(this));
    }
    
    /// <summary>
    /// 修改手机号
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="phoneRegion"></param>
    public void UpdatePhone(string phone, string phoneRegion)
    {
        Phone = phone;
        PhoneRegion = phoneRegion;
        AddDomainEvent(new UserModifiedDomainEvent(this));
    }
}
