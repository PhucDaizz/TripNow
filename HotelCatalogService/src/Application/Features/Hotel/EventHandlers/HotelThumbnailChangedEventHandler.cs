using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using HotelCatalogService.Domain.Events.Hotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.EventHandlers
{
    public class HotelThumbnailChangedEventHandler : INotificationHandler<HotelThumbnailChangedEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public HotelThumbnailChangedEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(HotelThumbnailChangedEvent notification, CancellationToken cancellationToken)
        {
            await _integrationEventService.PublishAsync<HotelThumbnailChangedIntegrationEvent>(new HotelThumbnailChangedIntegrationEvent
                {
                    HotelId = notification.HotelId,
                    ImageUrl = notification.ImageUrl
                },
                "hotel-catalog.events",
                "topic",
                "hotel.thumbnail_changed",
                cancellationToken
                );
        }
    }
}
