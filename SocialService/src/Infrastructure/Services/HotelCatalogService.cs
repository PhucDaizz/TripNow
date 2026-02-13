using Microsoft.AspNetCore.Http;
using Nexus.BuildingBlocks.Model;
using SocialService.Application.Contracts;
using System.Net.Http.Json;

namespace SocialService.Infrastructure.Services
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

        public async Task<bool> IsHotelExisting(Guid hotelId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Hotel/hotel-existing?hotelId={hotelId}", cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken: cancellationToken);
                    return apiResponse?.Data ?? false;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
