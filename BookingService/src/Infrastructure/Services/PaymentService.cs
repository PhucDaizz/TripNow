using BookingService.Application.Contracts;
using Microsoft.AspNetCore.Http;

namespace BookingService.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<string> CreatePaymentLinkAsync(Guid bookingId, decimal amount, CancellationToken token)
        {
            return Task.FromResult($"http://localhost:5000/api/bookings/test-payment-success?bookingId={bookingId}");
        }
    }
}
