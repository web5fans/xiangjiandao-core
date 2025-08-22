using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminGlobalConfig;

/// <summary>
/// 修改提案配置
/// </summary>
[Tags("AdminGlobalConfig")]
[HttpPost("/api/v1/admin/global-config/modify-proposal-config")]
[Authorize(PolicyNames.Admin)]
public class AdminModifyProposalConfigEndpoint(IMediator mediator)
    : Endpoint<ModifyProposalConfigReq, ResponseData<bool>>
{
    public override async Task HandleAsync(ModifyProposalConfigReq req, CancellationToken ct)
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
public class ModifyProposalConfigReq
{
    /// <summary>
    /// 基金规模
    /// </summary> 
    public required int ProposalApprovalVotes { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public ModifyProposalConfigCommand ToCommand()
    {
        return new ModifyProposalConfigCommand
        { 
            ProposalApprovalVotes = ProposalApprovalVotes,
        };
    }
}
