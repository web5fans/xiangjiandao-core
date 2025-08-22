using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Endpoints.AdminUser;

namespace Xiangjiandao.Web.Application.Queries;

public class AdminUserQuery(ApplicationDbContext applicationDbContext, ILogger<AdminUserQuery> logger)
{
    /// <summary>
    /// 根据邮箱查询管理员信息
    /// </summary>
    /// <param name="email"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AdminUserVo?> GetAdminUserByEmail(string email, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("AdminUserQuery GetAdminUserByEmail");
        return await applicationDbContext.AdminUsers.AsNoTracking()
            .Where(x => x.Email == email)
            .Select(user => new AdminUserVo
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                PhoneRegion = user.PhoneRegion,
                Avatar = user.Avatar,
                Role = user.Role,
                Special = user.Special,
                SecretData = user.SecretData
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
    
    /// <summary>
    /// 根据Id查询管理员信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AdminUserDataVo?> GetAdminUserById(AdminUserId id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("AdminUserQuery GetAdminUserByEmail");
        return await applicationDbContext.AdminUsers.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(user => new AdminUserDataVo
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                PhoneRegion = user.PhoneRegion,
                Avatar = user.Avatar,
                Role = user.Role,
                Special = user.Special
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 管理员账号列表
    /// </summary>
    public async Task<PagedData<AdminUserDataVo>> AdminPage(AdminUserPageReq req, CancellationToken cancellationToken)
    {
        logger.LogInformation("管理员账号列表");
        return await applicationDbContext.AdminUsers.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(user => new AdminUserDataVo
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                PhoneRegion = user.PhoneRegion,
                Avatar = user.Avatar,
                Role = user.Role,
                Special = user.Special
            })
            .ToPagedDataAsync(req.PageNum, req.PageSize, countTotal: true, cancellationToken: cancellationToken);
    }
    
    /// <summary>
    /// 管理员账号列表
    /// </summary>
    public async Task<PagedData<AdminUserDataVo>> AdminPage(AdminUserPageRequest req, CancellationToken cancellationToken)
    {
        logger.LogInformation("管理员账号列表");
        return await applicationDbContext.AdminUsers.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(user => new AdminUserDataVo
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                PhoneRegion = user.PhoneRegion,
                Avatar = user.Avatar,
                Role = user.Role,
                Special = user.Special
            })
            .ToPagedDataAsync(req.PageNum, req.PageSize, countTotal: true, cancellationToken: cancellationToken);
    }
    
    /// <summary>
    /// 根据手机号查询管理员信息
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="phoneRegion"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AdminUserVo?> GetAdminUserByPhone(string phone, string phoneRegion, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("AdminUserQuery GetAdminUserByEmail");
        return await applicationDbContext.AdminUsers.AsNoTracking()
            .Where(x =>x.Phone == phone && x.PhoneRegion == phoneRegion)
            .Select(user => new AdminUserVo
            {
                Id = user.Id,
                Email = user.Email,
                PhoneRegion = user.PhoneRegion,
                Phone = user.Phone,
                Avatar = user.Avatar,
                Role = user.Role,
                Special = user.Special,
                SecretData = user.SecretData
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}