using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.BannerAggregate;

namespace Xiangjiandao.Infrastructure.Repositories;

/// <summary>
/// Banner仓储接口
/// </summary>
public interface IBannerRepository : IRepository<Banner, BannerId>
{
    Task<int> LatestOrder(CancellationToken cancellationToken = default!);

    Task<List<Banner>> GetByIds(List<BannerId> ids, CancellationToken cancellationToken = default!);
}

/// <summary>
/// Banner仓储接口
/// </summary>
public class BannerRepository(ApplicationDbContext context)
    : RepositoryBase<Banner, BannerId, ApplicationDbContext>(context), IBannerRepository
{
    /// <summary>
    /// 获取最大排序
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> LatestOrder(CancellationToken cancellationToken = default!)
    {
        var hasAny = await context.Banners.AnyAsync(cancellationToken);

        if (!hasAny)
        {
            return 0;
        }

        return await context.Banners.MaxAsync(x => x.Sort, cancellationToken);
    }

    /// <summary>
    /// 按主键ID批量获取
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<Banner>> GetByIds(List<BannerId> ids, CancellationToken cancellationToken = default!)
    {
        return await context.Banners.Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}
