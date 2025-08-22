using MediatR;
using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.DomainEvents;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.DomainEventHandlers;

/// <summary>
/// 稻米发放事件
/// </summary>
/// <param name="mediator"></param>
/// <param name="logger"></param>
public class ScoreDistributeRecordCreatedDomainEventHandler(
    IMediator mediator,
    ILogger<ScoreDistributeRecordCreatedDomainEventHandler> logger) : IDomainEventHandler<ScoreDistributeRecordCreatedDomainEvent>
{
    public async Task Handle(ScoreDistributeRecordCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("ScoreDistributeRecordCreatedDomainEventHandler: {ScoreDistributeRecordId}", domainEvent.ScoreDistributeRecord.Id);
        var scoreDistributeRecord = domainEvent.ScoreDistributeRecord;
        await mediator.Send(new CreateScoreRecordCommand()
        {
            UserId = scoreDistributeRecord.UserId,
            Score = scoreDistributeRecord.Score,
            Type = ScoreSourceType.AdminDistribution,
            Reason = ScoreSourceType.AdminDistribution.GetDesc()
        });
        
        // 刷新积分统计
        await mediator.Send(new IssueGlobalConfigPointCommand()
        {
            IssuePointsScale = scoreDistributeRecord.Score,
        });
    }
    
}