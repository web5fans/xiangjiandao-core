using FastEndpoints;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminUser;

/// <summary>
/// 创建新的管理员用户 
/// </summary>
/// <param name="query"></param>
/// <param name="mediator"></param>
/// <param name="loginUser"></param>
[Tags("AdminUsers")]
[FastEndpoints.HttpPost("/api/v1/admin/admin-user/create")]
[Authorize(PolicyNames.AdminOnly)]
public class CreateNewAdminUserEndpoint(
    AdminUserQuery query,
    IMediator mediator,
    ILoginUser loginUser): Endpoint<CreateAdminUserRequest, ResponseData<AdminUserCreateVo>>
{
    public override async Task HandleAsync(CreateAdminUserRequest req,CancellationToken ct)
    {
        var user = await query.GetAdminUserByPhone(req.Phone, req.PhoneRegion,ct);
        // 账号已存在
        if (user != null)
        {
            var data = new[]
            {
                new ErrorDataVo()
                {
                    ErrorCode = "PhoneValidator",
                    ErrorMessage = "手机号已被添加",
                    PropertyName = "Phone"
                }
            };
            throw new KnownException("手机号已被添加", 400, data);
        }

        var login = await query.GetAdminUserById(new AdminUserId(loginUser.Id), ct);
        if (login == null)
        {
            throw new KnownException("管理员不存在");
        }
        if (login.Role != RoleType.Admin)
        {
            throw new KnownException("您没有权限操作新增");
        }
        var command = req.ToCommand();
        await SendAsync(await mediator.Send(command, ct).AsSuccessResponseData(), cancellation: ct);
    }
}


/// <summary>
/// 新增后台用户
/// </summary>
public record CreateAdminUserRequest
{
    
    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// 手机区号 默认86
    /// </summary>
    public string PhoneRegion { get; set; } = "86";
    
    /// <summary>
    /// 用户角色 1-管理员；2-运营
    /// </summary>
    public RoleType Role { get; set; } = RoleType.Unknown;
    
    /// <summary>
    /// 请求转换成命令
    /// </summary>
    public CreateAdminUserCommand ToCommand()
    {
        return new CreateAdminUserCommand(Role, Phone, PhoneRegion);
    }
}

public class CreateAdminUserRequestValidator : AbstractValidator<CreateAdminUserRequest>
{
    
    public CreateAdminUserRequestValidator()
    {
        RuleFor(u => u.Phone)
            .NotEmpty()
            .WithMessage("手机号不能为空");
    }
}