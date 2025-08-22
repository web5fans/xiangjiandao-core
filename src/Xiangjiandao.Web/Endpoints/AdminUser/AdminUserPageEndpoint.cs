using FastEndpoints;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminUser;

public class AdminUserPageEndpoint(
    AdminUserQuery query) : Endpoint<AdminUserPageRequest,ResponseData<PagedData<AdminUserDataVo>>>
{

    public override void Configure()
    {
        Post("/api/v1/admin/admin-user/page");
        Policies(PolicyNames.AdminOnly);
        Description(x=>x.WithTags("AdminUsers"));
    }
    
    public override async Task HandleAsync(AdminUserPageRequest req, CancellationToken cancellationToken)
    {
        var result = await query.AdminPage(req, cancellationToken);
        await SendAsync(result.AsSuccessResponseData(), cancellation: cancellationToken);
    }
    
}


/// <summary>
/// 后台管理员账号列表分页查询请求
/// </summary>
public record AdminUserPageRequest
{
    /// <summary>
    ///  分页页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// 分页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
}
