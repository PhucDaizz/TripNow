using MediatR;

namespace HotelCatalogService.Domain.Common
{
    public interface IDomainEvent : INotification
    {
        public Guid EventId { get; init; }
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
