namespace BookingService.Application.DTOs.Payment
{
    public class CreatePaymentLinkRequest
    {
        public PaymentProvider providerBank { get; set; }
        public string BookingId { get; set; }
        public double MoneyToPay { get; set; }
        public Guid? PayerUserId { get; set; }
    }
}
