using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// Create 公告命令
/// </summary>
public class CreateInformationCommand : ICommand<InformationId>
{
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
public class CreateInformationCommandHandler(
    InformationRepository repository,
    ILogger<CreateInformationCommandHandler> logger
) : ICommandHandler<CreateInformationCommand, InformationId>
{
    public async Task<InformationId> Handle(
        CreateInformationCommand command,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("CreateInformationCommand Handling");
        var information = Information.Create(command.Name, command.AttachId);
        await repository.AddAsync(information, cancellationToken);
        return  information.Id;
    }
}