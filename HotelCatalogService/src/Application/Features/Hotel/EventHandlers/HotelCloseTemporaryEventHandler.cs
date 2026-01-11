using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using HotelCatalogService.Domain.Enum;
using HotelCatalogService.Domain.Events.Hotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.EventHandlers
{
    public class HotelCloseTemporaryEventHandler : INotificationHandler<HotelCloseTemporaryEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public HotelCloseTemporaryEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(HotelCloseTemporaryEvent notification, CancellationToken cancellationToken)
        {
            var hotelStatusChangedEvent = new HotelStatusChangedEvent
            {
                HotelId = notification.HotelId,
                IsClosed = true,
                FromDate = notification.FromDate,
                ToDate = notification.ToDate
            };
            await _integrationEventService.PublishAsync<HotelStatusChangedEvent>(hotelStatusChangedEvent, "hotel-catalog.events", "topic", "hotel.close.temporary", cancellationToken);
        }
    }
}
