using MediatR;

namespace BookingService.Domain.Common
{
    public interface IDomainEvent : INotification
    {
        public Guid EventId { get; init; }
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
