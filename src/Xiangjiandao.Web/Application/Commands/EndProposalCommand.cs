using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 结束提案命令
/// </summary>
public class EndProposalCommand : ICommand<bool>
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class EndProposalCommandHandler(
    IProposalRepository repository,
    IGlobalConfigRepository globalConfigRepository,
    ILogger<EndProposalCommandHandler> logger
) : ICommandHandler<EndProposalCommand, bool>
{
    public async Task<bool> Handle(EndProposalCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("EndProposalCommand Handling");
        var globalConfig = await globalConfigRepository.LastValidConfigAsync(cancellationToken);
        if (globalConfig is null)
        {
            throw new KnownException("全局配置不存在");
        }

        var toEndProposal = await repository.GetAsync(command.ProposalId, cancellationToken);
        if (toEndProposal is null)
        {
            throw new KnownException("提案不存在");
        }

        toEndProposal.EndProposal(passThreshold: globalConfig.ProposalApprovalVotes);

        return true;
    }
}