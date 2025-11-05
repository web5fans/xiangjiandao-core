using NetCorePal.Extensions.Domain;

namespace Xiangjiandao.Domain.AggregatesModel.AppAggregate;

/// <summary>
/// 应用 Id
/// </summary>
public partial record AppId : IGuidStronglyTypedId;

/// <summary>
/// 应用
/// </summary>
public class App : Entity<AppId>, IAggregateRoot
{
    protected App()
    {
    }

    /// <summary>
    /// 应用名称
    /// </summary> 
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 应用描述
    /// </summary> 
    public string Desc { get; private set; } = string.Empty;

    /// <summary>
    /// 应用图标
    /// </summary> 
    public string Logo { get; private set; } = string.Empty;

    /// <summary>
    /// 应用排序
    /// </summary> 
    public int Sort { get; private set; }

    /// <summary>
    /// 应用链接
    /// </summary> 
    public string Link { get; private set; } = string.Empty;

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
    /// 创建应用
    ///</summary>
    public static App Create(
        string name,
        string desc,
        string logo,
        string link
    )
    {
        var instance = new App
        {
            Name = name,
            Desc = desc,
            Logo = logo,
            Link = link,
        };
        return instance;
    }

    /// <summary>
    /// 编辑应用
    /// </summary>
    public void Modify(
        string name,
        string desc,
        string logo,
        string link
    )
    {
        Name = name;
        Desc = desc;
        Logo = logo;
        Link = link;
    }

    /// <summary>
    /// 设置应用序号 
    /// </summary>
    public void SetSort(int sort)
    {
        Sort = sort;
    }

    /// <summary>
    /// 软删除应用
    /// </summary>
    public bool Delete()
    {
        Deleted = true;
        return true;
    }
}