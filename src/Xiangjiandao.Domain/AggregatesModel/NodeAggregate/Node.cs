using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Domain.AggregatesModel.NodeAggregate;

/// <summary>
/// 节点 Id
/// </summary>
public partial record NodeId : IGuidStronglyTypedId;

/// <summary>
/// 节点
/// </summary>
public class Node : Entity<NodeId>, IAggregateRoot
{
    protected Node()
    {
    }

    /// <summary>
    /// 节点用户 Id
    /// </summary>
    public UserId UserId { get; private set; } = null!;

    /// <summary>
    /// 节点用户 Did
    /// </summary>
    public string UserDid { get; private set; } = string.Empty;

    /// <summary>
    /// 节点 Logo
    /// </summary> 
    public string Logo { get; private set; } = string.Empty;

    /// <summary>
    /// 节点名称
    /// </summary> 
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 节点描述
    /// </summary> 
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary> 
    public int Sort { get; private set; }

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
    /// 创建节点
    ///</summary>
    public static Node Create(
        UserId userId,
        string userDid,
        string logo,
        string name,
        string description,
        int sort
    )
    {
        if (description.Length > 500)
        {
            throw new KnownException("节点描述长度超出限制");
        }

        var instance = new Node
        {
            UserDid = userDid,
            UserId = userId,
            Logo = logo,
            Name = name,
            Description = description,
            Sort = sort,
        };
        return instance;
    }

    /// <summary>
    /// 软删除节点
    /// </summary>
    public bool Delete()
    {
        Deleted = true;
        return true;
    }

    /// <summary>
    /// 修改节点
    /// </summary>
    public void Modify(
        UserId userId,
        string userDid,
        string logo, 
        string name, 
        string description
        )
    {
        if (description.Length > 500)
        {
            throw new KnownException("节点描述长度超出限制");
        }

        UserId = userId;
        UserDid = userDid;
        Logo = logo;
        Name = name;
        Description = description;
    }

    /// <summary>
    /// 设置序号
    /// </summary>
    public void SetOrder(int sort)
    {
        Sort = sort;
    }
}