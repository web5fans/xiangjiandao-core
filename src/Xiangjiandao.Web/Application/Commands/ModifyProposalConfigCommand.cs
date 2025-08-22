using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 修改提案配置
/// </summary>
public class ModifyProposalConfigCommand : ICommand<bool>
{
    /// <summary>
    /// 提案通过阈值
    /// </summary>
    public required int ProposalApprovalVotes { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class ModifyProposalConfigCommandHandler(
    IGlobalConfigRepository repository,
    ILogger<ModifyProposalConfigCommandHandler> logger
) : ICommandHandler<ModifyProposalConfigCommand, bool>
{
    public async Task<bool> Handle(ModifyProposalConfigCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("ModifyProposalConfigCommand Handling");
        var globalConfig = await repository.LastValidConfigAsync(cancellationToken);
        if (globalConfig is null)
        {
            throw new KnownException("全局配置不存在");
        }

        globalConfig.ModifyProposalConfig(proposalApprovalVotes: command.ProposalApprovalVotes);
        return true;
    }
}