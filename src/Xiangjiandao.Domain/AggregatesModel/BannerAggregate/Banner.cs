using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Primitives;

namespace Xiangjiandao.Domain.AggregatesModel.BannerAggregate;

/// <summary>
/// 横幅 Id
/// </summary>
public partial record BannerId : IGuidStronglyTypedId;


/// <summary>
/// 横幅
/// </summary>
public class Banner : Entity<BannerId>, IAggregateRoot
{
    protected Banner()
    {
    }

    /// <summary>
    /// Banner 文件 Id
    /// </summary> 
    public string BannerFileId { get; private set; } = string.Empty;

    /// <summary>
    /// 链接地址
    /// </summary> 
    public string LinkAddress { get; private set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary> 
    public int Sort { get; private set; }

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
    /// 创建横幅
    ///</summary>
    public static Banner Create(
       string bannerFileId,
       string linkAddress,
       int sort
    )
    {
        var instance = new Banner
        {
           BannerFileId = bannerFileId,
           LinkAddress = linkAddress,
           Sort = sort,
           CreatedAt = DateTimeOffset.Now,
        };
        return instance;
    }

    /// <summary>
    /// 软删除横幅
    /// </summary>
    public bool Delete()
    {
        CheckDeletedWithException();
        Deleted = true;
        return true;
    }
    
    /// <summary>
    /// 修改横幅
    /// </summary>
    /// <param name="bannerFileId"></param>
    /// <param name="linkAddress"></param>
    /// <returns></returns>
    public bool Modify(
        string bannerFileId,
        string linkAddress)
    {
        CheckDeletedWithException();
        
        BannerFileId = bannerFileId;
        LinkAddress = linkAddress;
        return true;
    }
    
    /// <summary>
    /// 更新横幅排序
    /// </summary>
    /// <param name="sort"></param>
    /// <returns></returns>
    public bool UpdateSort(int sort)
    {
        CheckDeletedWithException();
        Sort = sort;
        return true;
    }
    /// <summary>
    /// 检查横幅是否删除
    /// </summary>
    /// <exception cref="KnownException"></exception>
    private void CheckDeletedWithException()
    {
        if (Deleted)
        {
            throw new KnownException("banner does not exist");
        }
    }
    
}
