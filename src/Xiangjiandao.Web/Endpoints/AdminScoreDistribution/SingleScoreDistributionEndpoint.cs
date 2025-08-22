using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Domain.AggregatesModel.ScoreDistributeRecordAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminScoreDistribution;

/// <summary>
/// 单个稻米发放
/// </summary>
/// <param name="mediator"></param>
[Tags("ScoreDistributions")]
[HttpPost("/api/v1/admin/score-distribution/single")]
[Authorize(PolicyNames.AdminOnly)]
public class SingleScoreDistributionEndpoint(
    IMediator mediator,
    ILoginUser loginUser,
    IAliYunSMSVerifyCodeUtils sMsVerifyCodeUtils,
    AdminUserQuery adminUserQuery): Endpoint<SingleScoreDistributionReq, ResponseData<ScoreDistributeRecordId>>
{
    public override async Task HandleAsync(SingleScoreDistributionReq req, CancellationToken ct)
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
        
        var command = req.ToCommand(adminUser.Phone);
        await SendAsync( await mediator.Send(command, ct).AsSuccessResponseData(), cancellation: ct);
    }
}