using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Infrastructure.Repositories;

public interface IUserRepository : IRepository<User, UserId>
{
    Task<List<User>> GetByPhoneOrEmailAsync(List<string> userPhoneOrEmails,
        CancellationToken cancellationToken = default);

    Task<User?> FindByPhoneAsync(string phone, CancellationToken cancellationToken);

    Task<User?> FindByPhoneOrEmail(string email, string phone, CancellationToken cancellationToken);
    
    Task<User?> FindByPhoneOrEmail(string phoneOrEmail, CancellationToken cancellationToken);
}

public class UserRepository(ApplicationDbContext context)
    : RepositoryBase<User, UserId, ApplicationDbContext>(context), IUserRepository
{
    public async Task<List<User>> GetByPhoneOrEmailAsync(List<string> userPhoneOrEmails,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Where(x => userPhoneOrEmails.Contains(x.Email) || userPhoneOrEmails.Contains(x.Phone))
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> FindByPhoneAsync(string phone, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Phone == phone, cancellationToken);
    }

    public async Task<User?> FindByPhoneOrEmail(string email, string phone, CancellationToken cancellationToken)
    {
        User? result = null;
        if (!string.IsNullOrEmpty(phone))
        {
            result = await context.Users.FirstOrDefaultAsync(
                predicate: user => user.Phone == phone,
                cancellationToken: cancellationToken
            );
        }

        if (!string.IsNullOrEmpty(email))
        {
            result ??= await context.Users.FirstOrDefaultAsync(
                predicate: user => user.Email == email,
                cancellationToken: cancellationToken
            );
        }

        return result;
    }
    
    public async Task<User?> FindByPhoneOrEmail(string phoneOrEmail, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(user => user.Phone == phoneOrEmail || user.Email == phoneOrEmail,cancellationToken);
    }
}