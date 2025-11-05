using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.AppAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 修改应用命令
/// </summary>
public class ModifyAppCommand : ICommand<bool>
{
    /// <summary>
    /// 应用 Id
    /// </summary>
    public required AppId AppId { get; set; }

    /// <summary>
    /// 应用名称
    /// </summary> 
    public required string Name { get; set; }

    /// <summary>
    /// 应用描述
    /// </summary> 
    public required string Desc { get; set; }

    /// <summary>
    /// 应用图标
    /// </summary> 
    public required string Logo { get; set; }

    /// <summary>
    /// 应用链接
    /// </summary> 
    public required string Link { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class ModifyAppCommandHandler(
    IAppRepository appRepository,
    ILogger<CreateAppCommandHandler> logger
) : ICommandHandler<ModifyAppCommand, bool>
{
    public async Task<bool> Handle(ModifyAppCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("ModifyAppCommand Handling");
        var app = await appRepository.GetAsync(command.AppId, cancellationToken);
        if (app is null)
        {
            logger.LogInformation("App not found, AppId: {AppId}", command.AppId);
            throw new KnownException("应用未找到");
        }

        app.Modify(
            name: command.Name,
            desc: command.Desc,
            logo: command.Logo,
            link: command.Link
        );
        return true;
    }
}