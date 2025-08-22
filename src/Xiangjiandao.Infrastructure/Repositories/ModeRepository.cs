using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.MedalAggregate;

namespace Xiangjiandao.Infrastructure.Repositories;

public interface IMedalRepository : IRepository<Medal, MedalId>
{
}

public class MedalRepository(ApplicationDbContext context)
    : RepositoryBase<Medal, MedalId, ApplicationDbContext>(context), IMedalRepository
{
}
