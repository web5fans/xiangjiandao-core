using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Domain.Dto;

public class UserInfoDto
{
    public UserId Id { get; set; } = default!;
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
    /// 用户头像
    /// </summary> 
    public string Avatar { get; set; } = string.Empty;

    /// <summary>
    /// 用户昵称
    /// </summary> 
    public string NickName { get; set; } = string.Empty;
}