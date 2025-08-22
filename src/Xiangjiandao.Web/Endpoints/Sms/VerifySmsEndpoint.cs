using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.Sms;

/// <summary>
/// 校验短信验证码
/// </summary>
[Tags("Sms")]
[HttpPost("/api/v1/sms/verify")]
[AllowAnonymous]
public class VerifySmsEndpoint(IAliYunSMSVerifyCodeUtils aliYunSmsVerifyCodeUtils)
    : Endpoint<VerifySmsRequest, ResponseData<bool>>
{
    public override async Task HandleAsync(VerifySmsRequest req, CancellationToken cancellationToken)
    {
        await SendAsync(await aliYunSmsVerifyCodeUtils.VerifyAsync(req.PhoneRegion, req.Phone, req.Code,
            req.CodeType.GetDesc(),
            cancellationToken: cancellationToken).AsSuccessResponseData(), cancellation: cancellationToken);
    }
}

public class VerifySmsRequest
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

    /// <summary>
    /// 验证码
    /// </summary>
    public string Code { get; set; } = string.Empty;
}