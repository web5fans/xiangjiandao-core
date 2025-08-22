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
/// 普通用户翻页查询
/// </summary>
[Tags("UserManages")]
[HttpPost("/api/v1/admin/user-manage/user/page")]
[Authorize(PolicyNames.Admin)]
public class UserPageEndpoint(
    UserQuery query): Endpoint<UserPageReq, ResponseData<PagedData<UserPageVo>>>
{
    public override async Task HandleAsync(UserPageReq req, CancellationToken ct)
    {
        
        await SendAsync( await query.UserPage(req, ct).AsSuccessResponseData(), cancellation: ct);
    }
}