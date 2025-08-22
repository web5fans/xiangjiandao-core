using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 修改基金会配置
/// </summary>
public class ModifyFoundationInfoCommand : ICommand<bool>
{
    /// <summary>
    /// 基金规模
    /// </summary> 
    public required long FundScale { get; set; }

    /// <summary>
    /// 基金会公开信息文件
    /// </summary> 
    public required List<string> FoundationPublicDocument { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class ModifyFoundationInfoCommandHandler(
    IGlobalConfigRepository repository,
    ILogger<ModifyFoundationInfoCommandHandler> logger
) : ICommandHandler<ModifyFoundationInfoCommand, bool>
{
    public async Task<bool> Handle(ModifyFoundationInfoCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("ModifyFoundationInfoCommand Handling");
        var globalConfig = await repository.LastValidConfigAsync(cancellationToken: cancellationToken);
        if (globalConfig is null)
        {
            throw new KnownException("全局配置不存在");
        }

        globalConfig.ModifyFoundationInfo(
            fundScale: command.FundScale,
            foundationPublicDocument: command.FoundationPublicDocument
        );

        return true;
    }
}