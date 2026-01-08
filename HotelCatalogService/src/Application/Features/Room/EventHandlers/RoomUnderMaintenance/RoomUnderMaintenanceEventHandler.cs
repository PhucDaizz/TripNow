using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Domain.Events.Room;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.EventHandlers.RoomUnderMaintenance
{
    public class RoomUnderMaintenanceEventHandler : INotificationHandler<RoomUnderMaintenanceEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public RoomUnderMaintenanceEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(RoomUnderMaintenanceEvent notification, CancellationToken cancellationToken)
        {
            
            var integrationEvent = new RoomUnderMaintenanceDto
            {
                RoomTypeId = notification.roomTypeId,
                FromDate = notification.fromDate,
                ToDate = notification.toDate
            };

            await _integrationEventService.PublishAsync<RoomUnderMaintenanceDto>(integrationEvent, "hotel-catalog.events", "topic", "room.maintain", cancellationToken);
        }
    }
}
