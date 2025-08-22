using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Domain.Enums;

namespace Xiangjiandao.Web.Application.Vo;

/// <summary>
/// 管理员信息
/// </summary>
public class AdminUserVo
{
    /// <summary>
    /// 管理员Id
    /// </summary>
    public AdminUserId Id { get; set; } = default!;
    
    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 手机号
    /// </summary> 
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 手机区号
    /// </summary> 
    public string PhoneRegion { get; set; } = string.Empty;
    
    /// <summary>
    /// 头像
    /// </summary>
    public string Avatar { get; set; } = string.Empty;
    
    /// <summary>
    /// 用户角色 1-管理员；2-运营
    /// </summary>
    public RoleType Role { get; set; } = RoleType.Unknown;
    
    /// <summary>
    /// 是否特殊账号超级管理员
    /// </summary>
    public bool Special { get; set; } = false;
    
    /// <summary>
    /// 密码摘要
    /// </summary>
    public SecretData SecretData { get; set; } = default!;
}