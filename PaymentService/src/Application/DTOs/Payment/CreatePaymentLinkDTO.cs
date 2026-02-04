namespace PaymentService.Application.DTOs.Payment
{
    public class CreatePaymentLinkDTO
    {
        public ProviderBank providerBank { get; set; }
        public string BookingId { get; set; }
        public double MoneyToPay { get; set; }
    }
}
