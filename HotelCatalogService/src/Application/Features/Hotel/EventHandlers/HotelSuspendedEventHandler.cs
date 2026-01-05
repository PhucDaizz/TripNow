using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using HotelCatalogService.Domain.Events.Hotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.EventHandlers
{
    public class HotelSuspendedEventHandler : INotificationHandler<HotelSuspendedEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public HotelSuspendedEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(HotelSuspendedEvent notification, CancellationToken cancellationToken)
        {
            var hotelRejectedDto = new SuspendHotelEvent
            {
                OwnerId = notification.OwnerId,
                HotelName = notification.HotelName,
                Reason = notification.Reason
            };

            await _integrationEventService.PublishAsync<SuspendHotelEvent>(hotelRejectedDto, "hotel-catalog.events", "topic", "hotel.suspend", cancellationToken);
        }
    }
}
