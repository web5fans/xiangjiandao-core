using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 下架提案命令
/// </summary>
public class TakeOffProposalCommand : ICommand<bool>
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class TakeOffProposalCommandHandler(
    IProposalRepository repository,
    ILogger<TakeOffProposalCommandHandler> logger
) : ICommandHandler<TakeOffProposalCommand, bool>
{
    public async Task<bool> Handle(TakeOffProposalCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("TakeOffProposalCommand Handling");
        var toTakeOffProposal = await repository.GetAsync(command.ProposalId, cancellationToken);
        if (toTakeOffProposal is null)
        {
            throw new KnownException("提案不存在");
        }
        
        toTakeOffProposal.TakeOff();
        return true;
    }
}