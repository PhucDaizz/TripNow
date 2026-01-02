using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.EventHandlers.BookingCancelled
{
    public class BookingCancelledEvent : INotification
    {
        public Guid BookingId { get; set; }
    }
}
