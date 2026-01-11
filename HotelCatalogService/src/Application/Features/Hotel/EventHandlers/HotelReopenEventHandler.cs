using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using HotelCatalogService.Domain.Events.Hotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.EventHandlers
{
    public class HotelReopenEventHandler : INotificationHandler<HotelReopenEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public HotelReopenEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(HotelReopenEvent notification, CancellationToken cancellationToken)
        {
            var hotelStatusChangedEvent = new HotelStatusChangedEvent
            {
                HotelId = notification.hotelId,
                IsClosed = false,
            };
            await _integrationEventService.PublishAsync<HotelStatusChangedEvent>(hotelStatusChangedEvent, "hotel-catalog.events", "topic", "hotel.reopen", cancellationToken);
        }
    }
}
