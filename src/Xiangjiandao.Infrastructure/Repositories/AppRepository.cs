using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.AppAggregate;

namespace Xiangjiandao.Infrastructure.Repositories;

public interface IAppRepository : IRepository<App, AppId>
{
    /// <summary>
    /// 通过 Id 列表查找应用列表
    /// </summary>
    Task<List<App>> FindListByIds(List<AppId> ids, CancellationToken cancellationToken);
}

public class AppRepository(ApplicationDbContext context)
    : RepositoryBase<App, AppId, ApplicationDbContext>(context), IAppRepository
{
    /// <summary>
    /// 通过 Id 列表查找应用列表
    /// </summary>
    public async Task<List<App>> FindListByIds(List<AppId> ids, CancellationToken cancellationToken)
    {
        return await context.Apps
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}
