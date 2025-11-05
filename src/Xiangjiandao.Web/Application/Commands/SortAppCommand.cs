using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.AppAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 修改应用命令
/// </summary>
public class SortAppCommand : ICommand<bool>
{
    /// <summary>
    /// 应用 Id 列表
    /// </summary>
    public required List<AppId> AppIds { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class SortAppCommandHandler(
    IAppRepository appRepository,
    ILogger<CreateAppCommandHandler> logger
) : ICommandHandler<SortAppCommand, bool>
{
    public async Task<bool> Handle(SortAppCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("SortAppCommand Handling");
        var appDict = (await appRepository.FindListByIds(
                ids: command.AppIds,
                cancellationToken: cancellationToken
            ))
            .ToDictionary(x => x.Id, x => x);

        for (var i = 0; i < appDict.Count; i++)
        {
            if (appDict.TryGetValue(command.AppIds[i], out var app))
            {
                app.SetSort(i);
            }
        }

        return true;
    }
}