using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Domain.DomainEvents;

/// <summary>
/// 用户修改领域事件
/// </summary>
public record UserModifiedDomainEvent(User User) : IDomainEvent;