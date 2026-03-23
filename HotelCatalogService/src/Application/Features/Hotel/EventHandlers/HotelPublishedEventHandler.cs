using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel.Event;
using HotelCatalogService.Domain.Events.Hotel;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.EventHandlers
{
    public class HotelPublishedEventHandler : INotificationHandler<HotelPublishedEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public HotelPublishedEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(HotelPublishedEvent notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new HotelIndexedIntegrationEvent
            {
                HotelId       = notification.Id,
                Name          = notification.Name,
                Description   = notification.Description,
                City          = notification.City,
                Street        = notification.Street,
                Country       = notification.Country,
                Rating        = notification.Rating,
                StartingPrice = notification.StartingPrice,
                AmenityNames  = notification.AmenityNames,
                RoomTypes     = notification.RoomTypes
                                    .Select(rt => new HotelRoomTypeSummary
                                    {
                                        Name        = rt.Name,
                                        BasePrice   = rt.BasePrice,
                                        Capacity    = rt.Capacity,
                                        SizeM2      = rt.SizeM2,
                                        Description = rt.Description,
                                        CancellationPolicyDescription = rt.CancellationPolicyDescription
                                    }).ToList(),
                ThumbnailUrl  = notification.ThumbnailUrl
            };

            await _integrationEventService.PublishAsync(
                integrationEvent          : integrationEvent,
                exchange                  : "hotel-catalog.events",
                exchangeType              : "topic",
                routingKey                : "hotel.published",
                cancellationToken         : cancellationToken);
        }
    }
}