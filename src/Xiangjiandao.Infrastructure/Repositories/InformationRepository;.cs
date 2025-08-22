using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;

namespace Xiangjiandao.Infrastructure.Repositories;

public interface IInformationRepository : IRepository<Information, InformationId>
{
}

public class InformationRepository(ApplicationDbContext context)
    : RepositoryBase<Information, InformationId, ApplicationDbContext>(context), IInformationRepository
{
    /// <summary>
    /// 通过 Id 列表查找公告列表
    /// </summary>
    public async Task<List<Information>> GetByIds(List<InformationId> ids, CancellationToken cancellationToken)
    {
        return await context.Informations
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}
