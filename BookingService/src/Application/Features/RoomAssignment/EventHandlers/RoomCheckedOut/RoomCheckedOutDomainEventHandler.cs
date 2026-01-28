using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.HotelCatalog;
using BookingService.Domain.Events.RoomAssignment;
using MediatR;

namespace BookingService.Application.Features.RoomAssignment.EventHandlers.RoomCheckedOut
{
    public class RoomCheckedOutDomainEventHandler : INotificationHandler<RoomCheckedOutDomainEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public RoomCheckedOutDomainEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(RoomCheckedOutDomainEvent notification, CancellationToken cancellationToken)
        {
            var roomCheckOutDto = new RoomCheckedOutEventDto
            {
                RoomId = notification.RoomId
            };

            await _integrationEventService.PublishAsync<RoomCheckedOutEventDto>(
                roomCheckOutDto,
                "booking.events",
                "topic",
                "room.checkout",
                cancellationToken);
        }
    }
}
