namespace Xiangjiandao.Web.Application.Req;

/// <summary>
/// 后台管理员账号列表分页查询请求
/// </summary>
public record AdminUserPageReq
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