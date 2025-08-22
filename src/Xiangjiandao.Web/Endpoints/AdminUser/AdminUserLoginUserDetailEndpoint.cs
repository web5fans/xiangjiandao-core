using FastEndpoints;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminUser;

/// <summary>
/// 后台-获取当前登录用户的详细信息
/// </summary>
/// <param name="query"></param>
/// <param name="loginUser"></param>
public class AdminUserLoginUserDetailEndpoint(
    AdminUserQuery query,
    ILoginUser loginUser) : EndpointWithoutRequest<ResponseData<AdminUserDataVo?>>
{

    public override void Configure()
    {
        Post("/api/v1/admin/admin-user/login-user-detail");
        Policies(PolicyNames.Admin);
        Description(x=>x.WithTags("AdminUsers"));
    }


    public override async Task HandleAsync( CancellationToken cancellationToken)
    {
        var user = await query.GetAdminUserById(new AdminUserId(loginUser.Id), cancellationToken);
        await SendAsync(user.AsSuccessResponseData(), cancellation: cancellationToken);
    }
}
