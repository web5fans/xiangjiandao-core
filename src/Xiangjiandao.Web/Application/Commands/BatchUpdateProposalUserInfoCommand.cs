using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 批量更新提案用户相关信息命令
/// </summary>
public class BatchUpdateProposalUserInfoCommand : ICommand<bool>
{
    /// <summary>
    /// 用户 Id
    /// </summary>
    public required UserId UserId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class BatchUpdateProposalUserInfoCommandHandler(
    IUserRepository userRepository,
    IProposalRepository proposalRepository,
    ILogger<BatchUpdateProposalUserInfoCommandHandler> logger
) : ICommandHandler<BatchUpdateProposalUserInfoCommand, bool>
{
    public async Task<bool> Handle(BatchUpdateProposalUserInfoCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("BatchUpdateProposalUserInfoCommand Handling");
        var user = await userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogError("User not found, UserId: {UserId}", command.UserId);
            throw new KnownException("用户未找到");
        }

        var proposals = await proposalRepository.GetByUserId(userId: command.UserId, cancellationToken);
        foreach (var proposal in proposals)
        {
            proposal.ModifyUserInfo(
                initiatorName: user.NickName,
                initiatorEmail: user.Email,
                initiatorAvatar: user.Avatar
            );
        }

        return true;
    }
}