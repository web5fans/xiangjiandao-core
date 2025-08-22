using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Primitives;

namespace Xiangjiandao.Domain.AggregatesModel.GlobalConfigAggregate;

/// <summary>
/// 全局配置 Id
/// </summary>
public partial record GlobalConfigId : IGuidStronglyTypedId;

/// <summary>
/// 全局配置
/// </summary>
public class GlobalConfig : Entity<GlobalConfigId>, IAggregateRoot
{
    protected GlobalConfig()
    {
    }

    /// <summary>
    /// 基金规模
    /// </summary> 
    public long FundScale { get; private set; }

    /// <summary>
    /// 发行稻米规模
    /// </summary> 
    public long IssuePointsScale { get; private set; }

    /// <summary>
    /// 基金会公开信息文件
    /// </summary> 
    public List<string> FoundationPublicDocument { get; private set; } = new List<string>();

    /// <summary>
    /// 提案通过票数
    /// </summary> 
    public int ProposalApprovalVotes { get; private set; }

    /// <summary>
    /// 并发控制
    /// </summary>
    public RowVersion RowVersion { get; private set; } = new RowVersion(0);

    /// <summary>
    /// 创建时间
    /// </summary> 
    public DateTime CreatedAt { get; private set; } = DateTime.MinValue;

    /// <summary>
    /// 创建者
    /// </summary> 
    public string CreatedBy { get; private set; } = string.Empty;

    /// <summary>
    /// 更新时间
    /// </summary> 
    public UpdateTime UpdatedAt { get; private set; } = new UpdateTime(DateTimeOffset.MinValue);

    /// <summary>
    /// 更新者
    /// </summary> 
    public string UpdatedBy { get; private set; } = string.Empty;

    /// <summary>
    /// 是否删除
    /// </summary> 
    public Deleted Deleted { get; private set; } = new Deleted(false);

    /// <summary>
    /// 修改基金会配置
    /// </summary>
    public void ModifyFoundationInfo(
        long fundScale,
        List<string> foundationPublicDocument
    )
    {
        if (fundScale <= 0)
        {
            throw new KnownException("非法的基金会规模");
        }

        FundScale = fundScale;
        FoundationPublicDocument = foundationPublicDocument;
    }

    /// <summary>
    /// 发行积分
    /// </summary>
    /// <param name="points"></param>
    public void IssuePoints(long points)
    {
        if (points <= 0)
        {
            throw new KnownException("非法的稻米发放数量");
        }

        IssuePointsScale += points;
    }

    /// <summary>
    /// 修改提案配置
    /// </summary>
    public void ModifyProposalConfig(int proposalApprovalVotes)
    {
        ProposalApprovalVotes = proposalApprovalVotes;
    }

    /// <summary>
    /// 软删除全局配置
    /// </summary>
    public bool Delete()
    {
        Deleted = true;
        return true;
    }
}