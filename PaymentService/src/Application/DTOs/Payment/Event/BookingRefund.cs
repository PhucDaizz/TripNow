using MediatR;

namespace PaymentService.Application.DTOs.Payment.Event
{
    public class BookingRefund: INotification
    {
        public Guid BookingId { get; set; }
        public decimal RefundAmount { get; set; }
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
    }
}
