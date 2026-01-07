using BookingService.Domain.Common;

namespace BookingService.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
