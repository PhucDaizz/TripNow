using Microsoft.AspNetCore.Http;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.Contracts;
using PaymentService.Application.DTOs.HotelCatalog;
using System.Net.Http.Json;

namespace PaymentService.Infrastructure.Services
{
    public class HotelCatalogService : IHotelCatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HotelCatalogService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HotelSummaryDto?> GetHotelSummary(Guid hotelId, CancellationToken token = default)
        {
            var response = await _httpClient.GetAsync($"/api/Hotel/{hotelId}/summary");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<HotelSummaryDto?>>(cancellationToken: token);
                if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                {
                    return apiResponse.Data;
                }
            }

            return null;
        }
    }
}
