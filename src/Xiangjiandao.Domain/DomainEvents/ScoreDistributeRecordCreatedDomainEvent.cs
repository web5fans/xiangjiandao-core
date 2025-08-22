using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.AggregatesModel.ScoreDistributeRecordAggregate;

namespace Xiangjiandao.Domain.DomainEvents;

/// <summary>
/// 发放稻米记录创建领域事件
/// </summary>
/// <param name="ScoreDistributeRecord"></param>
public record ScoreDistributeRecordCreatedDomainEvent(ScoreDistributeRecord ScoreDistributeRecord) : IDomainEvent;