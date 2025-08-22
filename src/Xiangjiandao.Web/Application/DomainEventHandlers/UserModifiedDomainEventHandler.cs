using MediatR;
using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.DomainEvents;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.DomainEventHandlers;

/// <summary>
/// 用户修改邻域事件处理器
/// </summary>
public class UserModifiedDomainEventHandler(
    IMediator mediator,
    ILogger<UserModifiedDomainEventHandler> logger
) : IDomainEventHandler<UserModifiedDomainEvent>
{
    public async Task Handle(UserModifiedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("UserModifiedDomainEvent Handling");

        // 更新提案用户相关信息
        var batchUpdateProposalUserInfoCommand = new BatchUpdateProposalUserInfoCommand
        {
            UserId = notification.User.Id,
        };
        await mediator.Send(batchUpdateProposalUserInfoCommand, cancellationToken);

        var batchUpdateMedalUserInfoCommand = new BatchUpdateMedalUserInfoCommand
        {
            UserId = notification.User.Id,
        };
        await mediator.Send(batchUpdateMedalUserInfoCommand, cancellationToken);
    }
}