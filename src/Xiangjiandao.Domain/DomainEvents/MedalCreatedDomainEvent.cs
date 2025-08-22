using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.AggregatesModel.MedalAggregate;
using Xiangjiandao.Domain.Dto;

namespace Xiangjiandao.Domain.DomainEvents;

/// <summary>
/// 勋章创建事件
/// </summary>
/// <param name="Medal"></param>
/// <param name="UserInfoDtos"></param>
public record MedalCreatedDomainEvent(Medal Medal, List<UserInfoDto> UserInfoDtos) : IDomainEvent; 