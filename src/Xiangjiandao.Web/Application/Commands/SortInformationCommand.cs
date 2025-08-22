using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 排序公告命令
/// </summary>
public class SortInformationCommand : ICommand<bool>
{
    /// <summary>
    /// 公告 Id 列表
    /// </summary>
    public required List<InformationId> InformationIds { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class SortInformationCommandHandler(
    InformationRepository repository,
    ILogger<SortInformationCommandHandler> logger
) : ICommandHandler<SortInformationCommand, bool>
{
    public async Task<bool> Handle(SortInformationCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("SortInformationCommand Handling");
        var ids = command.InformationIds.Distinct().ToList();
        var informationList = await repository.GetByIds(ids, cancellationToken);
        var dict = informationList.ToDictionary(x => x.Id, x => x);
        if (dict.Count != ids.Count)
        {
            throw new KnownException("非法的公告列表，可能部分公告以不存在");
        }
        for (var i = 0; i < command.InformationIds.Count; i++)
        {
            if (dict.TryGetValue(command.InformationIds[i], out var information))
            {
                information.SetSort(i);
            }
        }

        return true;
    }
}