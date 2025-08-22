using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminProposal;

/// <summary>
/// 后台提案翻页查询
/// </summary>
[Tags("AdminProposal")]
[HttpPost("/api/v1/admin/proposal/page")]
[Authorize(PolicyNames.Admin)]
public class AdminProposalPageEndpoint(
    ProposalQuery query
) : Endpoint<AdminProposalPageReq, ResponseData<PagedData<AdminProposalPageVo>>>
{
    public override async Task HandleAsync(AdminProposalPageReq req, CancellationToken ct)
    {
        await SendAsync(
            response: await query.AdminPage(req, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}