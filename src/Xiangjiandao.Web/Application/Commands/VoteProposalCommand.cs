using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 给提案投票
/// </summary>
public class VoteProposalCommand : ICommand<bool>
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }

    /// <summary>
    /// 用户 Id
    /// </summary>
    public required UserId UserId { get; set; }

    /// <summary>
    /// 投票选择
    /// </summary>
    public required VoteType Choose { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class VoteProposalCommandHandler(
    IProposalRepository repository,
    IUserRepository userRepository,
    ILogger<VoteProposalCommandHandler> logger
) : ICommandHandler<VoteProposalCommand, bool>
{
    public async Task<bool> Handle(VoteProposalCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("VoteProposalCommand Handling");

        var user = await userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            throw new KnownException("未找到该用户");
        }

        if (user.Disable)
        {
            throw new KnownException("该用户已被禁用");
        }

        if (!user.NodeUser)
        {
            throw new KnownException("非节点用户不能投票提案");
        }

        var toVoteProposal = await repository.GetAsync(command.ProposalId, cancellationToken);
        if (toVoteProposal is null)
        {
            throw new KnownException("提案未找到");
        }

        toVoteProposal.VoteProposal(command.UserId, command.Choose);

        return true;
    }
}