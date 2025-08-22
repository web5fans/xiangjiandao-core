using Xiangjiandao.Domain.Enums;
using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Primitives;

namespace Xiangjiandao.Domain.AggregatesModel.InformationAggregate;

/// <summary>
/// 公告 Id
/// </summary>
public partial record InformationId : IGuidStronglyTypedId;

/// <summary>
/// 公告
/// </summary>
public class Information : Entity<InformationId>, IAggregateRoot
{
    protected Information()
    {
    }

    /// <summary>
    /// 公告名称
    /// </summary> 
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 附件 Id
    /// </summary> 
    public string AttachId { get; private set; } = string.Empty;

    /// <summary>
    /// 公告排序
    /// </summary>
    public int Sort { get; private set; }

    /// <summary>
    /// 并发乐观锁
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
    /// 创建公告
    ///</summary>
    public static Information Create(
        string name,
        string attachId,
        int sort = 0
    )
    {
        var instance = new Information
        {
            Name = name,
            AttachId = attachId,
            Sort = sort,
        };
        return instance;
    }

    /// <summary>
    /// 修改公告
    /// </summary>
    public void Modify(
        string name,
        string attachId
    )
    {
        Name = name;
        AttachId = attachId;
    }

    /// <summary>
    /// 设置公告排序
    /// </summary>
    public void SetSort(int sort)
    {
        if (sort < 0)
        {
            throw new KnownException("非法的排序序号");
        }

        Sort = sort;
    }

    /// <summary>
    /// 软删除公告
    /// </summary>
    public bool Delete()
    {
        Deleted = true;
        return true;
    }
}