using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 删除我的提案
/// </summary>
public class DeleteMyProposalCommand : ICommand<bool>
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }

    /// <summary>
    /// 用户 Id
    /// </summary>
    public required UserId UserId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class DeleteMyProposalCommandHandler(
    IProposalRepository repository,
    IUserRepository userRepository,
    ILogger<DeleteMyProposalCommandHandler> logger
) : ICommandHandler<DeleteMyProposalCommand, bool>
{
    public async Task<bool> Handle(DeleteMyProposalCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("DeleteMyProposalCommand Handling");
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
            throw new KnownException("非节点用户不能删除提案");
        }
        
        var toDeleteProposal = await repository.GetAsync(command.ProposalId, cancellationToken);
        if (toDeleteProposal is null)
        {
            throw new KnownException("提案未找到");
        }

        toDeleteProposal.DeleteMyProposal(userId: command.UserId);
        return true;
    }
}