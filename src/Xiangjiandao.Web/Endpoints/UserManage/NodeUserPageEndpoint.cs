using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.UserManage;

/// <summary>
/// 节点用户翻页查询
/// </summary>
[Tags("UserManages")]
[HttpPost("/api/v1/admin/user-manage/node-user/page")]
[Authorize(PolicyNames.Admin)]
public class NodeUserPageEndpoint(
    UserQuery query): Endpoint<NodeUserPageReq, ResponseData<PagedData<NodeUserPageVo>>>
{
    public override async Task HandleAsync(NodeUserPageReq req, CancellationToken ct)
    {
        
        await SendAsync( await query.NodeUserPage(req, ct).AsSuccessResponseData(), cancellation: ct);
    }
}