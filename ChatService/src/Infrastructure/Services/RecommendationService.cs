using ChatService.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Nexus.BuildingBlocks.Model;
using System.Net.Http.Json;

namespace ChatService.Infrastructure.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RecommendationService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<string>> GetHotelChatContextAsync(Guid hotelId, string userMessage, int limit = 3, CancellationToken cancellationToken = default)
        {
            var context = _httpContextAccessor.HttpContext;
            var bearerToken = context?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", bearerToken);
            }

            var requestBody = new
            {
                Message = userMessage,
                Limit = limit
            };

            var response = await _httpClient.PostAsJsonAsync($"/api/Rag/hotel/{hotelId}/context", requestBody, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<string>>>(cancellationToken);

                if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                {
                    return apiResponse.Data;
                }
            }

            return new List<string>();
        }
    }
}
