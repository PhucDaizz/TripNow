using MediatR;

namespace Application.DTOs.Payment
{
    public class RefundRequestCompleted: INotification
    {
        public Guid RefundId { get; set; }
        public Guid BookingId { get; set; }
        public Guid UserRefundId { get; set; }
        public decimal AmountRefund { get; set; }
    }
}
