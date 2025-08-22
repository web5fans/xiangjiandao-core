using Microsoft.EntityFrameworkCore;
using Xiangjiandao.Web.Endpoints.AdminGlobalConfig;
using Xiangjiandao.Web.Endpoints.GlobalConfig;

namespace Xiangjiandao.Web.Application.Queries;

/// <summary>
/// 全局配置相关查询
/// </summary>
public class GlobalConfigQuery(ApplicationDbContext dbContext)
{
    /// <summary>
    /// 全局配置详情
    /// </summary>
    public async Task<AdminGlobalConfigDetailVo?> Detail(CancellationToken cancellationToken)
    {
        return await dbContext.GlobalConfigs
            .OrderByDescending(config => config.CreatedAt)
            .Select(config => new AdminGlobalConfigDetailVo
            {
                FundScale = config.FundScale,
                IssuePointsScale = config.IssuePointsScale,
                FoundationPublicDocument = config.FoundationPublicDocument,
                ProposalApprovalVotes = config.ProposalApprovalVotes, 
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 基金会信息查询
    /// </summary>
    public async Task<FoundationInfoVo?> FoundationInfo(CancellationToken cancellationToken)
    {
        return await dbContext.GlobalConfigs
            .OrderByDescending(config => config.CreatedAt)
            .Select(config => new FoundationInfoVo
            {
                FundScale = config.FundScale,
                IssuePointsScale = config.IssuePointsScale,
                FoundationPublicDocument = config.FoundationPublicDocument,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}