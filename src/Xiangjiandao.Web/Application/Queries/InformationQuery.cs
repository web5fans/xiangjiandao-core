using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;
using Xiangjiandao.Web.Endpoints.AdminInformation;
using Xiangjiandao.Web.Endpoints.Information;

namespace Xiangjiandao.Web.Application.Queries;

/// <summary>
/// 公告相关查询
/// </summary>
public class InformationQuery(
    ApplicationDbContext dbContext
)
{
    /// <summary>
    /// 公告分页查询
    /// </summary>
    public async Task<PagedData<InformationPageVo>> Page(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        return await dbContext.Informations
            .OrderBy(info => info.Sort)
            .ThenByDescending(info => info.CreatedAt)
            .Select(info => new InformationPageVo
            {
                Id = info.Id,
                Name = info.Name,
                AttachId = info.AttachId,
                CreateAt = info.CreatedAt,
            })
            .ToPagedDataAsync(
                pageIndex: pageIndex,
                pageSize: pageSize,
                countTotal: true,
                cancellationToken: cancellationToken
            );
    }

    /// <summary>
    /// 后台公告分页查询
    /// </summary>
    public async Task<List<AdminInformationListVo>> AdminList(
        CancellationToken cancellationToken
    )
    {
        return await dbContext.Informations
            .OrderBy(info => info.Sort)
            .ThenByDescending(info => info.CreatedAt)
            .Select(info => new AdminInformationListVo
            {
                Id = info.Id,
                Name = info.Name,
                AttachId = info.AttachId,
                CreateAt = info.CreatedAt,
            })
            .ToListAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 后台公告详情
    /// </summary>
    public async Task<AdminInformationDetailVo?> AdminDetail(InformationId id, CancellationToken cancellationToken)
    {
        return await dbContext.Informations
            .Where(info => info.Id == id)
            .Select(info => new AdminInformationDetailVo
            {
                InformationId = info.Id,
                Name = info.Name,
                AttachId = info.AttachId,
                CreateAt = info.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    /// <summary>
    /// 公告详情
    /// </summary>
    public async Task<InformationDetailVo?> Detail(InformationId id, CancellationToken cancellationToken)
    {
        return await dbContext.Informations
            .Where(info => info.Id == id)
            .Select(info => new InformationDetailVo
            {
                InformationId = info.Id,
                Name = info.Name,
                AttachId = info.AttachId,
                CreateAt = info.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}