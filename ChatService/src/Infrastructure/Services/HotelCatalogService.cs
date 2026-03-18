using ChatService.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Nexus.BuildingBlocks.Model;
using System.Net.Http.Json;

namespace ChatService.Infrastructure.Services
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

        public async Task<bool> VerifyHotelOwnershipAsync(Guid hotelId, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            var bearerToken = context?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(bearerToken))
            {
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization = null;
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", bearerToken);

            var response = await _httpClient.GetAsync($"/api/Hotel/is-hotel-owner?hotelId={hotelId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken);
                if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                {
                    return apiResponse.Data;
                }
            }

            return false;

        }
    }
}
