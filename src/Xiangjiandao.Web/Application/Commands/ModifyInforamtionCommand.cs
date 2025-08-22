using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// Modify 公告命令
/// </summary>
public class ModifyInformationCommand : ICommand<InformationId>
{
    /// <summary>
    /// 公告 Id
    /// </summary>
    public required InformationId InformationId { get; set; }

    /// <summary>
    /// 公告名称
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 附件 Id
    /// </summary>
    public required string AttachId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class ModifyInformationCommandHandler(
    InformationRepository repository,
    ILogger<ModifyInformationCommandHandler> logger
) : ICommandHandler<ModifyInformationCommand, InformationId>
{
    public async Task<InformationId> Handle(
        ModifyInformationCommand command,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("ModifyInformationCommand Handling");
        var information = await repository.GetAsync(command.InformationId, cancellationToken);
        if (information is null)
        {
            throw new KnownException("公告未找到");
        }
        information.Modify(command.Name, command.AttachId);
        return information.Id;
    }
}