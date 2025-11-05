using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.AppAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 修改应用命令
/// </summary>
public class DeleteAppCommand : ICommand<bool>
{
    /// <summary>
    /// 应用 Id
    /// </summary>
    public required AppId AppId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class DeleteAppCommandHandler(
    IAppRepository appRepository,
    ILogger<CreateAppCommandHandler> logger
) : ICommandHandler<DeleteAppCommand, bool>
{
    public async Task<bool> Handle(DeleteAppCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("DeleteAppCommand Handling");
        var app = await appRepository.GetAsync(command.AppId, cancellationToken);
        if (app is null)
        {
            logger.LogInformation("App not found, AppId: {AppId}", command.AppId);
            throw new KnownException("应用未找到");
        }

        app.Delete();
        return true;
    }
}