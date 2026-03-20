using MediatR;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Domain.Common;

namespace RecommendationService.Infrastructure.Services
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
