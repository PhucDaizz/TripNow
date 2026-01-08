using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Domain.Events.Room;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.EventHandlers.RoomMaintenanceFinished
{
    public class RoomMaintenanceFinishedEventHandler : INotificationHandler<RoomMaintenanceFinishedEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public RoomMaintenanceFinishedEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(RoomMaintenanceFinishedEvent notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new RoomMaintenanceFinishedDto
            {
                RoomTypeId = notification.roomTypeId,
                FromDate = notification.oldStart,
                ToDate = notification.oldEnd
            };

            await _integrationEventService.PublishAsync<RoomMaintenanceFinishedDto>(integrationEvent, "hotel-catalog.events", "topic", "room.finished.maintain", cancellationToken);
        }
    }
}
