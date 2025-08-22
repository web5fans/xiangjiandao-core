using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.ScoreRecordAggregate;

namespace Xiangjiandao.Infrastructure.Repositories;

public interface IScoreRecordRepository : IRepository<ScoreRecord, ScoreRecordId>
{
}

public class ScoreRecordRepository(ApplicationDbContext context)
    : RepositoryBase<ScoreRecord, ScoreRecordId, ApplicationDbContext>(context), IScoreRecordRepository
{
}
