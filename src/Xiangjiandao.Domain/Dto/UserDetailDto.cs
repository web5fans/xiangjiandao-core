using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Domain.Dto;

/// <summary>
/// 用户详情
/// </summary>
public class UserDetailDto
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public required UserId Id { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary> 
    public required string Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary> 
    public required string Phone { get; set; }

    /// <summary>
    /// 手机区号
    /// </summary> 
    public required string PhoneRegion { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary> 
    public required string NickName { get; set; }

    /// <summary>
    /// 稻米
    /// </summary> 
    public required long Score { get; set; }

    /// <summary>
    /// 用户是否已禁用
    /// </summary>
    public required bool Disable { get; set; }

    /// <summary>
    /// 用户 Did
    /// </summary>
    public required string Did { get; set; }

    /// <summary>
    /// 是否是节点用户
    /// </summary>
    public required bool NodeUser { get; set; }
}