using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Events.Room;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.EventHandlers.RoomMovedToAnotherRoomType
{
    public class RoomMovedToAnotherRoomTypeEventHandler : INotificationHandler<RoomMovedToAnotherRoomTypeEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public RoomMovedToAnotherRoomTypeEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(RoomMovedToAnotherRoomTypeEvent notification, CancellationToken cancellationToken)
        {
            var roomMovedToAnotherRoomTypeDto = new RoomMovedToAnotherRoomTypeDto
            {
                OldRoomType = notification.oldRoomType,
                NewRoomType = notification.newRoomType
            };
            await _integrationEventService.PublishAsync<RoomMovedToAnotherRoomTypeDto>(roomMovedToAnotherRoomTypeDto, "hotel-catalog.events", "topic", "room.update", cancellationToken);
        }
    }
}
