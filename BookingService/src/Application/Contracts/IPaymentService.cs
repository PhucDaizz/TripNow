using BookingService.Application.DTOs.Payment;

namespace BookingService.Application.Contracts
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentLinkAsync(Guid bookingId, decimal amount, Guid? payerUserId, PaymentProvider paymentProvider, CancellationToken token);
    }
}
