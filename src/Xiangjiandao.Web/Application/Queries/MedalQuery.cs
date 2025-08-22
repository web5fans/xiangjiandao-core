using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Endpoints.AdminMedalDistribution;

namespace Xiangjiandao.Web.Application.Queries;

/// <summary>
/// 勋章查询
/// </summary>
/// <param name="dbContext"></param>
public class MedalQuery(ApplicationDbContext dbContext)
{
    /// <summary>
    /// 翻页查询
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PagedData<AdminMedalPageVo>> Page(AdminMedalPageReq req, CancellationToken cancellationToken)
    { 
        return await dbContext.Medals
            .OrderByDescending(medal => medal.CreatedAt)
            .Select(medal => new AdminMedalPageVo
            {
                Id = medal.Id,
                AttachId = medal.AttachId,
                Name = medal.Name,
                FileId = medal.FileId,
                Quantity = medal.Quantity,
            }).ToPagedDataAsync(req.PageNum, req.PageSize, countTotal: true, cancellationToken: cancellationToken);
    }
}