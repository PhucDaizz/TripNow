using PaymentService.Domain.Enum;

namespace PaymentService.Application.DTOs.Payment
{
    public class CreatePaymentLinkDTO
    {
        public PaymentProvider providerBank { get; set; }
        public string BookingId { get; set; }
        public double MoneyToPay { get; set; }
    }
}
