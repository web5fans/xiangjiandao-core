using Xiangjiandao.Domain.Enums;
using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;

/// <summary>
/// 提案 Id
/// </summary>
public partial record ProposalId : IGuidStronglyTypedId;

/// <summary>
/// 提案
/// </summary>
public class Proposal : Entity<ProposalId>, IAggregateRoot
{
    protected Proposal()
    {
    }

    /// <summary>
    /// 提案名称
    /// </summary> 
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 发起方名称
    /// </summary> 
    public UserId InitiatorId { get; private set; } = null!;

    /// <summary>
    /// 发起方 Did
    /// </summary>
    public string InitiatorDid { get; private set; } = string.Empty;

    /// <summary>
    /// 发起方域名
    /// </summary>
    public string InitiatorDomainName { get; private set; } = string.Empty;

    /// <summary>
    /// 发起方名称
    /// </summary> 
    public string InitiatorName { get; private set; } = string.Empty;

    /// <summary>
    /// 发起方邮箱
    /// </summary> 
    public string InitiatorEmail { get; private set; } = string.Empty;

    /// <summary>
    /// 发起方头像
    /// </summary> 
    public string InitiatorAvatar { get; private set; } = string.Empty;

    /// <summary>
    /// 投票截至时间
    /// </summary> 
    public DateTimeOffset EndAt { get; private set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// 附件 Id
    /// </summary> 
    public string AttachId { get; private set; } = string.Empty;

    /// <summary>
    /// 总投票数
    /// </summary> 
    public long TotalVotes { get; private set; }

    /// <summary>
    /// 反对票数
    /// </summary> 
    public long OpposeVotes { get; private set; }

    /// <summary>
    /// 同意票数
    /// </summary> 
    public long AgreeVotes { get; private set; }

    /// <summary>
    /// 提案状态
    /// </summary> 
    public ProposalStatus Status { get; private set; } = ProposalStatus.Unknown;

    /// <summary>
    /// 是否上架
    /// </summary>
    public bool OnShelf { get; private set; }

    /// <summary>
    /// 并发控制
    /// </summary>
    public RowVersion RowVersion { get; private set; } = new RowVersion(0);

    /// <summary>
    /// 投票记录列表
    /// </summary>
    public virtual ICollection<VoteRecord> Votes { get; private set; } = [];

    /// <summary>
    /// 创建时间
    /// </summary> 
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.MinValue;

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

    ///<summary>
    /// 创建提案
    ///</summary>
    public static Proposal Create(
        string name,
        UserId initiatorId,
        string initiatorDid,
        string initiatorDomainName,
        string initiatorName,
        string initiatorEmail,
        string initiatorAvatar,
        DateTimeOffset endAt,
        string attachId
    )
    {
        if (endAt <= DateTimeOffset.Now)
        {
            throw new KnownException("非法的提案结束时间");
        }

        var instance = new Proposal
        {
            Name = name,
            InitiatorId = initiatorId,
            InitiatorDid = initiatorDid,
            InitiatorDomainName = initiatorDomainName,
            InitiatorName = initiatorName,
            InitiatorEmail = initiatorEmail,
            InitiatorAvatar = initiatorAvatar,
            EndAt = endAt,
            AttachId = attachId,
            Status = ProposalStatus.Review,
            OnShelf = true,
            CreatedAt = DateTimeOffset.Now,
        };
        return instance;
    }

    /// <summary>
    /// 投票提案 
    /// </summary>
    public void VoteProposal(
        UserId userId,
        VoteType voteType
    )
    {
        // 已下架不允许投票
        if (!OnShelf)
        {
            throw new KnownException("提案已下架");
        }

        if (Status != ProposalStatus.Review)
        {
            throw new KnownException("提案已结束");
        }

        // 已投票不能继续投票
        var hasVoted = Votes.Any(record => record.UserId == userId);
        if (hasVoted)
        {
            throw new KnownException("您已投票");
        }

        var voteRecord = VoteRecord.Create(userId, voteType);
        Votes.Add(voteRecord);
        CalculateVotes();
    }

    /// <summary>
    /// 提案结束
    /// </summary>
    public void EndProposal(int passThreshold)
    {
        // 已下架的提案忽略
        if (!OnShelf)
        {
            return;
        }

        if (passThreshold < 0)
        {
            throw new KnownException("无效的提案通过票数");
        }

        if (AgreeVotes >= passThreshold)
        {
            Status = ProposalStatus.Pass;
            return;
        }

        Status = ProposalStatus.Oppose;
    }

    /// <summary>
    /// 下架提案
    /// </summary>
    public void TakeOff()
    {
        OnShelf = false;
    }

    /// <summary>
    /// 软删除提案
    /// </summary>
    public bool Delete()
    {
        Deleted = true;
        return true;
    }

    /// <summary>
    /// 软删除我的提案
    /// </summary>
    public void DeleteMyProposal(UserId userId)
    {
        if (userId != InitiatorId)
        {
            throw new KnownException("该用户不是提案发起方");
        }

        Deleted = true;
    }

    /// <summary>
    /// 计算投票
    /// </summary>
    private void CalculateVotes()
    {
        TotalVotes = Votes.Count(vote => vote.Choice != VoteType.Unknown);
        AgreeVotes = Votes.Count(vote => vote.Choice == VoteType.Agree);
        OpposeVotes = Votes.Count(vote => vote.Choice == VoteType.Oppose);
    }

    /// <summary>
    /// 修改提案用户信息
    /// </summary>
    public void ModifyUserInfo(
        string initiatorName,
        string initiatorEmail,
        string initiatorAvatar
    )
    {
        InitiatorName = initiatorName;
        InitiatorEmail = initiatorEmail;
        InitiatorAvatar = initiatorAvatar;
    }
}