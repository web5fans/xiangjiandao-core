using FastEndpoints;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminUser;

/// <summary>
/// 后台-用户重置密码 
/// </summary>
/// <param name="mediator"></param>
/// <param name="query"></param>
/// <param name="sMsVerifyCodeUtils"></param>
public class AdminUserResetPasswordEndpoint(
    [FromServices] IMediator mediator,
    [FromServices] AdminUserQuery query,
    [FromServices] IAliYunSMSVerifyCodeUtils sMsVerifyCodeUtils) : Endpoint<ResetAdminUserPasswordRequest, ResponseData<bool>>
{
    public override void Configure()
    {
        Post("/api/v1/admin/admin-user/reset-password");
        Description(x=>x.WithTags("AdminUsers"));
        AllowAnonymous();
    }

    public override async Task HandleAsync(ResetAdminUserPasswordRequest req,CancellationToken ct)
    {
        // 根据邮箱号查管理员
        var adminUser = await query.GetAdminUserByPhone(req.Phone, req.PhoneRegion, ct);
        if (adminUser is null)
        {
            var data = new[]
            {
                new ErrorDataVo()
                {
                    ErrorCode = "PhoneValidator",
                    ErrorMessage = "管理员账号不存在",
                    PropertyName = "Phone"
                }
            };
            throw new KnownException("管理员账号不存在", 400 , data);
        }
        
        // 验证邮箱收到的验证码
        var verify = await sMsVerifyCodeUtils.VerifyAsync(req.PhoneRegion, req.Phone, req.Code, CodeType.AdminUserResetPassword.GetDesc(),ct);
        if (!verify)
        {
            var data = new[]
            {
                new ErrorDataVo()
                {
                    ErrorCode = "VerifyCodeValidator",
                    ErrorMessage = "验证码错误或已过期",
                    PropertyName = "Code"
                }
            };
            throw new KnownException("验证码错误或已过期", 400, data);
        }

        await SendAsync(await mediator.Send(new ResetAdminUserPasswordCommand(req.PhoneRegion, req.Phone, req.NewPassword),
            ct).AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 后台管理员重置密码
/// </summary>
public record ResetAdminUserPasswordRequest
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
    /// 新密码
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// 确认新密码
    /// </summary>
    public string ConfirmNewPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// 邮箱收到的验证码
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

public class ResetAdminUserPasswordRequestValidator : AbstractValidator<ResetAdminUserPasswordRequest>
{
    public ResetAdminUserPasswordRequestValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("手机号不能为空");
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("新密码不能为空");
        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty()
            .WithMessage("确认密码不能为空");
        RuleFor(x => x.NewPassword)
            .Equal(x => x.ConfirmNewPassword)
            .WithMessage("新密码和确认密码不一致");
    }
}