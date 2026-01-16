using MediatR;

namespace HotelCatalogService.Application.Features.Promotion.EventHandlers.BookingCancelled
{
    public class BookingCancelledEvent : INotification
    {
        public Guid HotelId { get; set; }
        public Guid BookingId { get; set; }
        public string PromotionCode { get; set; }
    }
}
