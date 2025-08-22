using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminUser;

/// <summary>
/// 删除新的后台管理员账号
/// </summary>
/// <param name="query"></param>
/// <param name="mediator"></param>
/// <param name="loginUser"></param>
[Tags("AdminUsers")]
[FastEndpoints.HttpPost("/api/v1/admin/admin-user/delete")]
[Authorize(PolicyNames.AdminOnly)]
public class DeleteNewAdminUserEndpoint(
    AdminUserQuery query,
    IMediator mediator,
    ILoginUser loginUser): Endpoint<AdminUserIdRequest, ResponseData<bool>>
{
    public override async Task HandleAsync(AdminUserIdRequest req,CancellationToken ct)
    {
        var login = await query.GetAdminUserById(new AdminUserId(loginUser.Id), ct);
        if (login == null)
        {
            throw new KnownException("管理员不存在");
        }
        if (login.Role != RoleType.Admin)
        {
            throw new KnownException("您没有权限操作删除管理员/运营人员");
        }
        
        await SendAsync(await mediator.Send(new DeleteAdminUserCommand(req.Id), ct)
            .AsSuccessResponseData(), cancellation: ct);
    }
}


/// <summary>
/// 后台用户Id
/// </summary>
public record AdminUserIdRequest
{
    /// <summary>
    /// 后台用户Id
    /// </summary>
    public AdminUserId Id { get; set; } = default!;
}