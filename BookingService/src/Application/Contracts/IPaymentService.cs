using BookingService.Application.DTOs.Payment;

namespace BookingService.Application.Contracts
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentLinkAsync(Guid bookingId, decimal amount, PaymentProvider paymentProvider, CancellationToken token);
    }
}
