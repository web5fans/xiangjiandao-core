using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.GlobalConfigAggregate;

namespace Xiangjiandao.Infrastructure.Repositories;

public interface IGlobalConfigRepository : IRepository<GlobalConfig, GlobalConfigId>
{
    /// <summary>
    /// 最新的全局配置
    /// </summary>
    Task<GlobalConfig?> LastValidConfigAsync(CancellationToken cancellationToken);
}

public class GlobalConfigRepository(ApplicationDbContext context)
    : RepositoryBase<GlobalConfig, GlobalConfigId, ApplicationDbContext>(context), IGlobalConfigRepository
{
    /// <summary>
    /// 获取最新的全局配置
    /// </summary>
    public async Task<GlobalConfig?> LastValidConfigAsync(CancellationToken cancellationToken)
    {
        return await context.GlobalConfigs
            .OrderByDescending(globalConfig => globalConfig.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
