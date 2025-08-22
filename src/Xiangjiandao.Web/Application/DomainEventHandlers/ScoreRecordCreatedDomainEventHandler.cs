using MediatR;
using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.DomainEvents;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Application.DomainEventHandlers;

/// <summary>
/// 稻米记录创建事件
/// </summary>
public class ScoreRecordCreatedDomainEventHandler(
    IMediator mediator,
    ILogger<ScoreDistributeRecordCreatedDomainEventHandler> logger) : IDomainEventHandler<ScoreRecordCreatedDomainEvent>
{
    public async Task Handle(ScoreRecordCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("ScoreRecordCreatedDomainEventHandler: {ScoreRecordId}", domainEvent.ScoreRecord.Id);
        var scoreRecord = domainEvent.ScoreRecord;
        await mediator.Send(new UpdateUserScoreCommand(scoreRecord.UserId, scoreRecord.Score));
    }
}