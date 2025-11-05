using Microsoft.EntityFrameworkCore;
using Xiangjiandao.Web.Endpoints.AdminApp;
using Xiangjiandao.Web.Endpoints.App;

namespace Xiangjiandao.Web.Application.Queries;

/// <summary>
/// 应用相关查询
/// </summary>
public class AppQuery(
    ApplicationDbContext dbContext,
    ILogger<AppQuery> logger
)
{
    /// <summary>
    /// 前台获取排序的应用列表
    /// </summary>
    public async Task<List<AppListVo>> SortedList(CancellationToken cancellationToken)
    {
        logger.LogInformation("AppQuery::SortedList");
        return await dbContext.Apps
            .OrderBy(app => app.Sort)
            .ThenByDescending(app => app.CreatedAt)
            .Select(app => new AppListVo
            {
                AppId = app.Id,
                Name = app.Name,
                Desc = app.Desc,
                Logo = app.Logo,
                Link = app.Link,
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 后台获取排序的应用列表
    /// </summary>
    public async Task<List<AdminAppListVo>> AdminSortedList(CancellationToken cancellationToken)
    {
        logger.LogInformation("AppQuery::SortedList");
        return await dbContext.Apps
            .OrderBy(app => app.Sort)
            .ThenByDescending(app => app.CreatedAt)
            .Select(app => new AdminAppListVo
            {
                AppId = app.Id,
                Name = app.Name,
                Desc = app.Desc,
                Logo = app.Logo,
                Link = app.Link,
            })
            .ToListAsync(cancellationToken);
    }
}