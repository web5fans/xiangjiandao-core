using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.AppAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 创建应用命令
/// </summary>
public class CreateAppCommand : ICommand<AppId>
{
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
public class CreateAppCommandHandler(
    IAppRepository appRepository,
    ILogger<CreateAppCommandHandler> logger
) : ICommandHandler<CreateAppCommand, AppId>
{
    public async Task<AppId> Handle(CreateAppCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("CreateAppCommand Handling");
        var app = App.Create(
            name: command.Name,
            desc: command.Desc,
            logo: command.Logo,
            link: command.Link
        );
        await appRepository.AddAsync(app, cancellationToken);
        return app.Id;
    }
}