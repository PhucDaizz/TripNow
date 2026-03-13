using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Events.Hotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.EventHandlers
{
    public class HotelCreatedEventHandler : INotificationHandler<HotelCreatedEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public HotelCreatedEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(HotelCreatedEvent notification, CancellationToken cancellationToken)
        {
            await _integrationEventService.PublishAsync<DTOs.Hotel.HotelCreatedEvent>(
                    new DTOs.Hotel.HotelCreatedEvent
                    {
                        HotelId = notification.HotelId,
                        Name = notification.HotelName
                    },
                    "hotel-catalog.events",
                    "topic",
                    "hotel.created",
                    cancellationToken
                );
        }
    }
}
