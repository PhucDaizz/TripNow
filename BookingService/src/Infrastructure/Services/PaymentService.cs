using BookingService.Application.Contracts;
using BookingService.Application.DTOs.HotelCatalog;
using BookingService.Application.DTOs.Payment;
using Microsoft.AspNetCore.Http;
using Nexus.BuildingBlocks.Model;
using System.Net.Http.Json;

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

        public async Task<string> CreatePaymentLinkAsync(Guid bookingId, decimal amount, Guid? payerUserId, PaymentProvider paymentProvider, CancellationToken token)
        {
            var requestBody = new CreatePaymentLinkRequest
            {
                BookingId = bookingId.ToString(),
                MoneyToPay = (double)amount,
                PayerUserId = payerUserId,
                providerBank = paymentProvider
            };

            var response = await _httpClient.PostAsJsonAsync("api/Payment/payment-link", requestBody, token);

            if (response.IsSuccessStatusCode)
            {
                var paymentLink = await response.Content.ReadFromJsonAsync<ApiResponse<string>>(cancellationToken: token);
                return paymentLink.Data;
            }
            else
            {
                throw new Exception($"Failed to create payment link. Status code: {response.StatusCode}");
            }
        }
    }
}
