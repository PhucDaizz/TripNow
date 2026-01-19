namespace BookingService.Application.Contracts
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentLinkAsync(Guid bookingId, decimal amount, CancellationToken token);
    }
}
