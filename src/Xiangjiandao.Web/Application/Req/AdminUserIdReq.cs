using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;

namespace Xiangjiandao.Web.Application.Req;

/// <summary>
/// 后台用户Id
/// </summary>
public record AdminUserIdReq
{
    /// <summary>
    /// 后台用户Id
    /// </summary>
    public AdminUserId Id { get; set; } = default!;
}