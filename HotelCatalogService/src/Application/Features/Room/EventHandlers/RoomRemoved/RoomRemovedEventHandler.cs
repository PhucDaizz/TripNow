using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Domain.Events.Room;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.EventHandlers.RoomRemoved
{
    public class RoomRemovedEventHandler : INotificationHandler<RoomRemovedEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public RoomRemovedEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(RoomRemovedEvent notification, CancellationToken cancellationToken)
        {
            var typeRoomDeduct = new DeductRoomInventoryEvent
            {
                RoomTypeId = notification.RoomTypeId
            };

            await _integrationEventService.PublishAsync<DeductRoomInventoryEvent>(typeRoomDeduct, "hotel-catalog.events", "topic", "room.deduct", cancellationToken);
        }
    }
}
