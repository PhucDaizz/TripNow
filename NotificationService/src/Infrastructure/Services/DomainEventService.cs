using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Common;

namespace NotificationService.Infrastructure.Services
{
    public class DomainEventService : IDomainEventService
    {
        private readonly IMediator _mediator;

        public DomainEventService(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}
