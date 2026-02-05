using MediatR;

namespace PaymentService.Application.DTOs.Payment.Event
{
    public class BookingCompleted: INotification
    {
        public Guid BookingId { get; set; }
        public Guid HotelId { get; set; }
        public decimal Amount { get; set; }
    }
}
