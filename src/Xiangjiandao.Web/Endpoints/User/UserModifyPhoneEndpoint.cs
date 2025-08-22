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
/// 修改手机号
/// </summary>
[Tags("User")]
[HttpPost("/api/v1/user/modify-phone")]
[Authorize(PolicyNames.Client)]
public class UserModifyPhoneEndpoint( 
    IMediator mediator,
    ILoginUser loginUser,
    IAliYunSMSVerifyCodeUtils sMsVerifyCodeUtils
) : Endpoint<UserModifyPhoneReq, ResponseData<bool>>
{
    public override async Task HandleAsync(UserModifyPhoneReq req, CancellationToken ct)
    { 
        var userId = new UserId(loginUser.Id);
        
        // 验证手机收到的验证码
        var verify = await sMsVerifyCodeUtils.VerifyAsync(req.PhoneRegion, req.Phone, req.Code,req.CodeType.GetDesc(), ct);
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
/// 用户修改手机号
/// </summary>
public record UserModifyPhoneReq
{
    /// <summary>
    /// 手机区号, 默认86
    /// </summary>
    public string PhoneRegion { get; set; } = "86";
    
    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// 验证码类型 1-登录；2-重置密码；3-注册；4-修改邮箱
    /// </summary>
    public CodeType CodeType { get; set; } = CodeType.Unknown;
    
    /// <summary>
    /// 验证码
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    
    public ModifyUserPhoneCommand ToCommand(UserId userId)
    {
        return new ModifyUserPhoneCommand()
        {
            Id = userId,
            Phone = Phone,
            PhoneRegion = PhoneRegion
        };
    }
}

public class UserModifyPhoneReqValidator : AbstractValidator<UserModifyPhoneReq>
{
    public UserModifyPhoneReqValidator()
    {
        RuleFor(x => x.Phone).NotEmpty().WithMessage("手机号不能为空");
        RuleFor(x => x.Code).NotEmpty().WithMessage("验证码不能为空");
    }
}