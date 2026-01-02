using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Domain.Events.Room;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.EventHandlers.RoomCreated
{
    public class RoomCreatedEventHandler : INotificationHandler<RoomCreatedEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public RoomCreatedEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(RoomCreatedEvent notification, CancellationToken cancellationToken)
        {
            var roomDto = new AddRoomInventoryEvent
            {
                RoomtypeId = notification.RoomTypeId
            };

            await _integrationEventService.PublishAsync<AddRoomInventoryEvent>(roomDto, "hotel-catalog.events", "topic", "room.created", cancellationToken);
        }
    }
}