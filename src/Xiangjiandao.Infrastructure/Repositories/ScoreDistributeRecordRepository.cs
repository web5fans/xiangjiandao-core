using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.ScoreDistributeRecordAggregate;

namespace Xiangjiandao.Infrastructure.Repositories;

public interface IScoreDistributeRecordRepository : IRepository<ScoreDistributeRecord, ScoreDistributeRecordId>
{
}

public class ScoreDistributeRecordRepository(ApplicationDbContext context)
    : RepositoryBase<ScoreDistributeRecord, ScoreDistributeRecordId, ApplicationDbContext>(context), IScoreDistributeRecordRepository
{
}
