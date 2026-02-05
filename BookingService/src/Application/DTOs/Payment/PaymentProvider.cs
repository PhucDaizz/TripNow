namespace BookingService.Application.DTOs.Payment
{
    public enum PaymentProvider : byte
    {
        VNPay = 0,
        Momo = 1,
        Stripe = 2,
        Cash = 3
    }
}
