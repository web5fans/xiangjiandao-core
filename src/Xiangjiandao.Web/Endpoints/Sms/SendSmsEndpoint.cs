using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.Sms;

/// <summary>
/// 发送短信
/// </summary>
[Tags("Sms")]
[HttpPost("/api/v1/sms/send")]
[AllowAnonymous]
public class SendSmsEndpoint(IAliYunSMSVerifyCodeUtils aliYunSmsVerifyCodeUtils)
    : Endpoint<SendSmsRequest, ResponseData<bool>>
{
    public override async Task HandleAsync(SendSmsRequest req, CancellationToken cancellationToken)
    {
        await aliYunSmsVerifyCodeUtils.SendAsync(req.PhoneRegion, req.Phone, req.CodeType.GetDesc(),
            cancellationToken: cancellationToken);
        await SendAsync(true.AsSuccessResponseData(), cancellation: cancellationToken);
    }
}

public class SendSmsRequest
{
    /// <summary>
    /// 手机号区号
    /// </summary>
    public string PhoneRegion { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 验证码类型 1-登录；2-重置密码；3-注册；4-修改邮箱
    /// </summary>
    public CodeType CodeType { get; set; } = CodeType.Unknown;
}