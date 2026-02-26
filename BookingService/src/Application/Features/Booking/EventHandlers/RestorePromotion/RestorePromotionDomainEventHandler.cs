using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Booking.Event;
using BookingService.Domain.Events.Booking;
using MediatR;

namespace BookingService.Application.Features.Booking.EventHandlers.RestorePromotion
{
    public class RestorePromotionDomainEventHandler : INotificationHandler<RestorePromotionDomainEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public RestorePromotionDomainEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(RestorePromotionDomainEvent notification, CancellationToken cancellationToken)
        {
            await _integrationEventService.PublishAsync<RestorePromotionDto>(
                new RestorePromotionDto
                {
                    BookingId = notification.BookingId,
                    HotelId = notification.HotelId,
                    PromotionCode = notification.PromotionCode
                },
                "booking.events",
                "topic",
                "booking.cancelled",
                cancellationToken);
        }
    }
}
