using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.AggregatesModel.ScoreRecordAggregate;

namespace Xiangjiandao.Domain.DomainEvents;

/// <summary>
/// 稻米记录创建事件
/// </summary>
/// <param name="ScoreRecord"></param>
public record ScoreRecordCreatedDomainEvent(ScoreRecord ScoreRecord) : IDomainEvent;