using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.MedalAggregate;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminMedalDistribution;
/// <summary>
/// 创建banner
/// </summary>
[Tags("MedalDistributions")]
[HttpPost("/api/v1/admin/medal/create")]
[Authorize(PolicyNames.AdminOnly)]
public class CreateMedalEndpoint(
    IMediator mediator): Endpoint<CreateMedalReq, ResponseData<MedalId>>
{
    public override async Task HandleAsync(CreateMedalReq req, CancellationToken ct)
    {
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
        
        var command = req.ToCommand(userPhoneOrEmails.Distinct().ToList());
        await SendAsync( await mediator.Send(command, ct).AsSuccessResponseData(), cancellation: ct);
    }
}