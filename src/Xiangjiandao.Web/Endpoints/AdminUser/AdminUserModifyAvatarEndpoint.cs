using FastEndpoints;
using MediatR;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminUser;

/// <summary>
/// 后台-修改后台用户头像
/// </summary>
/// <param name="mediator"></param>
/// <param name="loginUser"></param>
public class AdminUserModifyAvatarEndpoint(
    IMediator mediator,ILoginUser loginUser) : Endpoint<ModifyAdminUserAvatarRequest, ResponseData<bool>>
{

    public override void Configure()
    {
        Post("/api/v1/admin/admin-user/modify-avatar");
        Policies(PolicyNames.Admin);
        Description(x=>x.WithTags("AdminUsers"));
    }


    public override async Task HandleAsync(ModifyAdminUserAvatarRequest req,CancellationToken cancellationToken)
    {
        if (loginUser == null)
        {
            throw new KnownException("未登录");
        }
        var command = req.ToCommand(new AdminUserId(loginUser.Id));
        await SendAsync(await mediator.Send(command, cancellationToken).AsSuccessResponseData(), cancellation: cancellationToken);
    }
}

/// <summary>
/// 修改后台用户头像
/// </summary>
public record ModifyAdminUserAvatarRequest
{
    /// <summary>
    /// 新头像
    /// </summary>
    public string Avatar { get; set; } = string.Empty;
    
    public ModifyAdminUserAvatarCommand ToCommand(AdminUserId id)
    {
        return new ModifyAdminUserAvatarCommand(id, Avatar);
    }
}