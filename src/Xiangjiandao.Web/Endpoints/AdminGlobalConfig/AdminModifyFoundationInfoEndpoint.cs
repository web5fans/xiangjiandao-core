using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminGlobalConfig;

/// <summary>
/// 修改基金会配置
/// </summary>
[Tags("AdminGlobalConfig")]
[HttpPost("/api/v1/admin/global-config/modify-foundation-info")]
[Authorize(PolicyNames.Admin)]
public class AdminModifyFoundationInfoEndpoint(IMediator mediator)
    : Endpoint<ModifyFoundationInfoReq, ResponseData<bool>>
{
    public override async Task HandleAsync(ModifyFoundationInfoReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 修改基金会配置请求
/// </summary>
public class ModifyFoundationInfoReq
{
    /// <summary>
    /// 基金规模
    /// </summary> 
    public required long FundScale { get; set; }

    /// <summary>
    /// 基金会公开信息文件
    /// </summary> 
    public required List<string> FoundationPublicDocument { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public ModifyFoundationInfoCommand ToCommand()
    {
        return new ModifyFoundationInfoCommand
        {
            FundScale = FundScale,
            FoundationPublicDocument = FoundationPublicDocument
        };
    }
}