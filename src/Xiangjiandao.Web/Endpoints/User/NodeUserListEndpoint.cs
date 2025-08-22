using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;

namespace Xiangjiandao.Web.Endpoints.User;

/// <summary>
/// 节点用户列表
/// </summary>
[Tags("User")]
[HttpPost("/api/v1/user/node-user-list")]
[AllowAnonymous]
public class NodeUserListEndpoint(
    UserQuery userQuery,
    ILogger<NodeUserListEndpoint> logger
) : EndpointWithoutRequest<ResponseData<List<NodeUserVo>>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await userQuery.NodeUserList(cancellationToken: ct);
        await SendAsync(
            response: result.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 节点用户列表响应
/// </summary>
public class NodeUserVo
{
    /// <summary>
    /// 用户 Id
    /// </summary>
    public required UserId Id { get; set; }

    /// <summary>
    /// DID
    /// </summary> 
    public required string Did { get; set; }
}