using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 发行积分命令
/// </summary>
public class IssueGlobalConfigPointCommand : ICommand<bool>
{
    /// <summary>
    /// 发行稻米规模
    /// </summary> 
    public required long IssuePointsScale { get; set; }
}

public class IssueGlobalConfigPointCommandHandler(
    IGlobalConfigRepository repository,
    ILogger<ModifyFoundationInfoCommandHandler> logger
) : ICommandHandler<IssueGlobalConfigPointCommand, bool>
{
    public async Task<bool> Handle(IssueGlobalConfigPointCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("IssueGlobalConfigPointCommand Handling");
        var globalConfig = await repository.LastValidConfigAsync(cancellationToken: cancellationToken);
        if (globalConfig is null)
        {
            throw new KnownException("全局配置不存在");
        }

        globalConfig.IssuePoints(command.IssuePointsScale);
        return true;
    }
}