using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminScoreDistribution;

/// <summary>
/// 批量发放稻米
/// </summary>
/// <param name="mediator"></param>
[Tags("ScoreDistributions")]
[HttpPost("/api/v1/admin/score-distribution/batch")]
[Authorize(PolicyNames.AdminOnly)]
public class BatchScoreDistributionEndpoint(
    IMediator mediator,
    ILoginUser loginUser,
    IAliYunSMSVerifyCodeUtils sMsVerifyCodeUtils,
    AdminUserQuery adminUserQuery): Endpoint<BatchScoreDistributionReq, ResponseData<bool>>
{
    public override async Task HandleAsync(BatchScoreDistributionReq req, 
        
        CancellationToken ct)
    {
        var adminUser = await adminUserQuery.GetAdminUserById(new AdminUserId(loginUser.Id), ct);
        if (adminUser == null)
        {
            throw new KnownException("管理员不存在");
        }
        // 校验验证码 手机验证码
        var verify = await sMsVerifyCodeUtils.VerifyAsync(adminUser.PhoneRegion, adminUser.Phone, req.Code,CodeType.AdminUserScoreDistribution.GetDesc(), ct);
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
        
        // 读取文件
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var storagePath = Path.Combine(basePath, "Data/File/");
        
        if (!Directory.Exists(storagePath))
        {
            throw new KnownException("文件不存在");
        }
        var stream = FileUtils.GetStream(storagePath,req.FileId);
        var userPhoneOrEmails = FileUtils.StreamUserPhoneOrEmail(stream);
        if (userPhoneOrEmails.Count == 0)
        {
            throw new KnownException("文件内容不可以为空");
        }
        var command = req.ToCommand(userPhoneOrEmails.Distinct().ToList(), adminUser.Phone);
        await SendAsync( await mediator.Send(command, ct).AsSuccessResponseData(), cancellation: ct);
    }
}