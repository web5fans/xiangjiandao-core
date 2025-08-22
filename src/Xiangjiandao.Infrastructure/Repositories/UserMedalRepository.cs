using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Infrastructure.Repositories;

public interface IUserMedalRepository : IRepository<UserMedal, UserMedalId>
{
    /// <summary>
    /// 通过用户 Id 查找用户勋章
    /// </summary>
    Task<List<UserMedal>> GetByUserId(UserId userId, CancellationToken cancellationToken);
}
public class UserMedalRepository (ApplicationDbContext context)
    : RepositoryBase<UserMedal, UserMedalId, ApplicationDbContext>(context), IUserMedalRepository
{
    /// <summary>
    /// 通过用户 Id 查找用户勋章
    /// </summary>
    public async Task<List<UserMedal>> GetByUserId(UserId userId, CancellationToken cancellationToken)
    {
        return await context.UserMedals
            .Where(userMedal => userMedal.UserId == userId)
            .ToListAsync(cancellationToken);
        
    }
}