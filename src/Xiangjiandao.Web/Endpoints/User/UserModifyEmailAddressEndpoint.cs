using FastEndpoints;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.User;

/// <summary>
/// 修改邮箱地址
/// </summary>
[Tags("User")]
[HttpPost("/api/v1/user/modify-email-address")]
[Authorize(PolicyNames.Client)]
public class UserModifyEmailAddressEndpoint( 
    IMediator mediator,
    ILoginUser loginUser,
    IEmailVerifyCodeUtils emailVerifyCodeUtils
) : Endpoint<UserModifyEmailAddressReq, ResponseData<bool>>
{
    public override async Task HandleAsync(UserModifyEmailAddressReq req, CancellationToken ct)
    { 
        var userId = new UserId(loginUser.Id);
        
        // 验证邮箱收到的验证码
        var verify = await emailVerifyCodeUtils.VerifyAsync(req.Email, req.Code, req.CodeType);
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
        
        var command = req.ToCommand(userId);
        await SendAsync(await mediator.Send(command, ct)
            .AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 用户修改邮箱地址
/// </summary>
public record UserModifyEmailAddressReq
{
    /// <summary>
    /// 邮箱地址
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 验证码类型 1-登录；2-重置密码；3-注册；4-修改邮箱
    /// </summary>
    public CodeType CodeType { get; set; } = CodeType.Unknown;
    
    /// <summary>
    /// 验证码
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    
    public ModifyUserEmailAddressCommand ToCommand(UserId userId)
    {
        return new ModifyUserEmailAddressCommand()
        {
            Id = userId,
            Email = Email
        };
    }
}

public class UserModifyEmailAddressReqValidator : AbstractValidator<UserModifyEmailAddressReq>
{
    public UserModifyEmailAddressReqValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("邮箱不能为空");
        RuleFor(x => x.Email).EmailAddress().WithMessage("邮箱格式不正确");
        RuleFor(x => x.Code).NotEmpty().WithMessage("验证码不能为空");
    }
}