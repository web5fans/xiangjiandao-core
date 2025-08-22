using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;

namespace Xiangjiandao.Web.Application.Vo;

public class AdminUserCreateVo
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public AdminUserId Id { get; set; } = default!;
    
    /// <summary>
    /// 初始密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
}