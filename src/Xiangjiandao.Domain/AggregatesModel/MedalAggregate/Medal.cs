using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.DomainEvents;
using Xiangjiandao.Domain.Dto;

namespace Xiangjiandao.Domain.AggregatesModel.MedalAggregate;

/// <summary>
/// 勋章 Id
/// </summary>
public partial record MedalId : IGuidStronglyTypedId;

/// <summary>
/// 勋章
/// </summary>
public class Medal : Entity<MedalId>, IAggregateRoot
{
    protected Medal()
    {
    }

    /// <summary>
    /// 封面Id
    /// </summary>
    public string AttachId { get; private set; } = string.Empty;

    /// <summary>
    /// 勋章名称
    /// </summary> 
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 获得用户文件 Id
    /// </summary> 
    public string FileId { get; private set; } = string.Empty;

    /// <summary>
    /// 发放用户数量
    /// </summary> 
    public long Quantity { get; private set; }

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
    /// 创建勋章
    ///</summary>
    public static Medal Create(
        string attachId,
        string name,
        string fileId,
        long quantity,
        List<UserInfoDto> userInfoDtos
    )
    {
        var instance = new Medal
        {
            AttachId = attachId,
            Name = name,
            FileId = fileId,
            Quantity = quantity,
        };
        instance.AddDomainEvent(new MedalCreatedDomainEvent(instance, userInfoDtos));
        return instance;
    }

    /// <summary>
    /// 软删除勋章
    /// </summary>
    public bool Delete()
    {
        Deleted = true;
        return true;
    }
}