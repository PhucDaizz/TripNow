using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using HotelCatalogService.Domain.Events.Hotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.EventHandlers
{
    public class HotelRejectedEventHandler : INotificationHandler<HotelRejectedEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public HotelRejectedEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(HotelRejectedEvent notification, CancellationToken cancellationToken)
        {
            var hotelRejectedDto = new RejectedHotelEvent
            {
                OwnerId = notification.OwnerId,
                HotelName = notification.HotelName,
                Reason = notification.Reason
            };

            await _integrationEventService.PublishAsync<RejectedHotelEvent>(hotelRejectedDto, "hotel-catalog.events", "topic", "hotel.rejected", cancellationToken);
        }
    }
}
