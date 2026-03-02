using ChatService.Domain.Common;

namespace ChatService.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
