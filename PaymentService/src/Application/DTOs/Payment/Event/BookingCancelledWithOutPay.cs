using MediatR;

namespace PaymentService.Application.DTOs.Payment.Event
{
    public class BookingCancelledWithOutPay: INotification
    {
        public Guid BookingId { get; set; }
        public string Reason { get; set; }
    }
}
