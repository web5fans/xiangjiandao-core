using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.Proposal;

/// <summary>
/// 创建提案
/// </summary>
[Tags("Proposal")]
[HttpPost("/api/v1/proposal/create")]
[Authorize(PolicyNames.Client)]
public class CreateProposalEndpoint(
    IMediator mediator,
    ILoginUser loginUser
) : Endpoint<CreateProposalReq, ResponseData<ProposalId>>
{
    public override async Task HandleAsync(CreateProposalReq req, CancellationToken ct)
    {
        var userId = new UserId(loginUser.Id);
        var command = req.ToCommand(userId);
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 创建提案请求
/// </summary>
public class CreateProposalReq
{
    /// <summary>
    /// 提案名称
    /// </summary> 
    public required string Name { get; set; }

    /// <summary>
    /// 投票截至时间
    /// </summary> 
    public required DateTimeOffset EndAt { get; set; }

    /// <summary>
    /// 附件 Id
    /// </summary> 
    public required string AttachId { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public CreateProposalCommand ToCommand(UserId userId)
    {
        return new CreateProposalCommand
        {
            Name = Name,
            InitiatorId = userId,
            EndAt = EndAt,
            AttachId = AttachId
        };
    }
}