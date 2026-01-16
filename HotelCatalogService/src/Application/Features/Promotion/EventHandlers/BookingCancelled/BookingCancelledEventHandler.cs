using HotelCatalogService.Application.Features.Promotion.Commands.RestorePromotion;
using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.EventHandlers.BookingCancelled
{
    public class BookingCancelledEventHandler : INotificationHandler<BookingCancelledEvent>
    {
        private readonly IMediator _mediator;

        public BookingCancelledEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(BookingCancelledEvent notification, CancellationToken cancellationToken)
        {
            var command = new RestorePromotionCommand { 
                HotelId = notification.HotelId,
                BookingId = notification.BookingId,
                PromotionCode = notification.PromotionCode
            };
            await _mediator.Send(command, cancellationToken);
        }
    }
}
