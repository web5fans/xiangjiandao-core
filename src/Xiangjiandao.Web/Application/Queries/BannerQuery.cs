using Microsoft.EntityFrameworkCore;
using Xiangjiandao.Web.Application.Vo;

namespace Xiangjiandao.Web.Application.Queries;

/// <summary>
/// Banner查询
/// </summary>
/// <param name="applicationDbContext"></param>
public class BannerQuery(ApplicationDbContext applicationDbContext)
{
    /// <summary>
    /// 获取Banner列表
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<AdminBannerVo>> List(CancellationToken cancellationToken)
    {
        return await applicationDbContext.Banners
            .OrderByDescending(x => x.Sort)
            .Select(x => new AdminBannerVo
            {
                Id = x.Id,
                BannerFileId = x.BannerFileId,
                LinkAddress = x.LinkAddress,
                Sort = x.Sort,
            }).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 获取Banner列表
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<BannerVo>> ListForC(CancellationToken cancellationToken)
    {
        return await applicationDbContext.Banners
            .OrderByDescending(x => x.Sort)
            .Select(x => new BannerVo
            {
                Id = x.Id,
                BannerFileId = x.BannerFileId,
                LinkAddress = x.LinkAddress,
            }).ToListAsync(cancellationToken);
    }
}