using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// Delete 公告命令
/// </summary>
public class DeleteInformationCommand : ICommand<InformationId>
{
    /// <summary>
    /// 公告 Id
    /// </summary>
    public required InformationId InformationId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class DeleteInformationCommandHandler(
    InformationRepository repository,
    ILogger<DeleteInformationCommandHandler> logger
) : ICommandHandler<DeleteInformationCommand, InformationId>
{
    public async Task<InformationId> Handle(
        DeleteInformationCommand command,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("DeleteInformationCommand Handling");
        var information = await repository.GetAsync(command.InformationId, cancellationToken);
        if (information is null)
        {
            throw new KnownException("公告未找到");
        }
        information.Delete();
        return information.Id;
    }
}
