using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;

namespace Xiangjiandao.Infrastructure.Repositories;

public interface IAdminUserRepository : IRepository<AdminUser,  AdminUserId>
{
    Task<AdminUser?> FindByPhoneAsync(string phone, string phoneRegion, CancellationToken cancellationToken);
}

public class AdminUserRepository(ApplicationDbContext context) : RepositoryBase<AdminUser, AdminUserId, ApplicationDbContext>(context), IAdminUserRepository
{
    /// <summary>
    /// 根据手机号查询
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="phoneRegion"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AdminUser?> FindByPhoneAsync(string phone, string phoneRegion, CancellationToken cancellationToken)
    {
        return await context.AdminUsers.FirstOrDefaultAsync(x => x.Phone == phone && x.PhoneRegion== phoneRegion , cancellationToken);
    }
}