using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.Email;

/// <summary>
/// 发送邮件
/// </summary>
[Tags("Email")]
[HttpPost("/api/v1/email/send")]
[AllowAnonymous]
public class SendEmailEndpoint(IEmailVerifyCodeUtils emailVerifyCodeUtils) : Endpoint<SendEmailRequest, ResponseData<bool>>
{

    public override async Task HandleAsync(SendEmailRequest req, CancellationToken cancellationToken)
    {
        // 发送邮件
        await emailVerifyCodeUtils.SendAsync(req.Name, req.Email, req.CodeType);

        await SendAsync(true.AsSuccessResponseData(), cancellation: cancellationToken);
    }
}


/// <summary>
/// 发送邮件
/// </summary>
public record SendEmailRequest
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 邮箱账号
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 验证码类型 1-登录；2-重置密码；3-注册；4-修改邮箱
    /// </summary>
    public CodeType CodeType { get; set; } = CodeType.Unknown;
}