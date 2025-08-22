using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.UserManage;

/// <summary>
/// 模糊查找未绑定的节点用户
/// </summary>
[Tags("UserManages")]
[HttpPost("/api/v1/admin/user-manage/user/unbound-node-user-search")]
[Authorize(PolicyNames.Admin)]
public class UnboundNodeUserSearchEndpoint(
    UserQuery query) : Endpoint<UnboundNodeUserSearchReq, ResponseData<List<UnboundNodeUserSearchVo>>>
{
    public override async Task HandleAsync(UnboundNodeUserSearchReq req, CancellationToken ct)
    {
        await SendAsync(await query.UnboundNodeUserSearch(req.PhoneOrEmail, ct).AsSuccessResponseData(),
            cancellation: ct);
    }
}

/// <summary>
/// 模糊查找未绑定的节点用户请求
/// </summary>
public record UnboundNodeUserSearchReq
{
    /// <summary>
    /// 手机号或邮箱
    /// </summary>
    public string PhoneOrEmail { get; set; } = string.Empty;
}

/// <summary>
/// 模糊查找未绑定的节点用户响应
/// </summary>
public record UnboundNodeUserSearchVo
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public UserId UserId { get; set; } = null!;

    /// <summary>
    /// 用户Id
    /// </summary>
    public string UserDid { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;
}