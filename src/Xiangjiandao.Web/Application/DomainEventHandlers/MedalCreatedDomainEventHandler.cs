using MediatR;
using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.DomainEvents;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.DomainEventHandlers;

public class MedalCreatedDomainEventHandler(
    IMediator mediator,
    ILogger<MedalCreatedDomainEventHandler> logger) : IDomainEventHandler<MedalCreatedDomainEvent>
{
    public async Task Handle(MedalCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("MedalCreatedDomainEventHandler: {MedalId}", domainEvent.Medal.Id);
        
        await mediator.Send(new CreateBatchUserMedalCommand()
        {
            MedalId = domainEvent.Medal.Id,
            AttachId = domainEvent.Medal.AttachId,
            Name = domainEvent.Medal.Name,
            Quantity = domainEvent.Medal.Quantity,
            UserInfoDtos = domainEvent.UserInfoDtos,
        }, cancellationToken);
    }
}