using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Endpoints.UserMedal;

namespace Xiangjiandao.Web.Application.Queries;

/// <summary>
/// 用户勋章查询
/// </summary>
/// <param name="dbContext"></param>
public class UserMedalQuery(ApplicationDbContext dbContext)
{
    /// <summary>
    /// B端查询勋章发放用户列表
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PagedData<AdminUserMedalPageVo>> Page(AdminUserMedalPageReq req, CancellationToken cancellationToken)
    {
        return await dbContext.UserMedals.AsNoTracking()
            .Where(userMedal => userMedal.MedalId == req.MedalId)
            .WhereIf(!string.IsNullOrEmpty(req.PhoneOrEmail), userMedal => userMedal.Phone.Contains(req.PhoneOrEmail) || userMedal.Email.Contains(req.PhoneOrEmail))
            .Select(userMedal => new AdminUserMedalPageVo
            {
                Avatar = userMedal.Avatar,
                NickName = userMedal.NickName,
                Phone = userMedal.Phone,
                PhoneRegion = userMedal.PhoneRegion,
                Email = userMedal.Email,
            }).ToPagedDataAsync(req.PageNum, req.PageSize, countTotal: true, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// C 端查询用户勋章列表
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PagedData<UserMedalPageVo>> Page(UserId userId, UserMedalPageReq req, CancellationToken cancellationToken)
    {
        return await dbContext.UserMedals
            .Where(um => um.UserId == userId)
            .OrderByDescending(um => um.GetTime)
            .Select(um => new UserMedalPageVo
            {
                MedalId = um.MedalId,
                AttachId = um.AttachId,
                Name = um.Name,
                GetTime = um.GetTime
            }).ToPagedDataAsync(req.PageNum, req.PageSize, countTotal: true, cancellationToken: cancellationToken);
    }
    
    
}