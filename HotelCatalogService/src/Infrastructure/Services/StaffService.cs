using HotelCatalogService.Application.Contracts;
using HotelCatalogService.Application.DTOs.StaffProfile;
using Microsoft.AspNetCore.Http;
using Nexus.BuildingBlocks.Model;
using System.Net.Http.Json;

namespace HotelCatalogService.Infrastructure.Services
{
    public class StaffService : IStaffService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StaffService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<StaffInfoDto?> GetStaffInfoAsync(string userId, CancellationToken token = default)
        {
            var context = _httpContextAccessor.HttpContext;
            var bearerToken = context?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(bearerToken))
            {
                return null;
            }

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", bearerToken);

            var response = await _httpClient.GetAsync("/api/Auth/staff-profile", token);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<StaffInfoDto>>(cancellationToken: token);

                return new StaffInfoDto
                {
                    UserId = apiResponse.Data.UserId,
                    HotelId = apiResponse.Data.HotelId,
                    Position = apiResponse.Data.Position
                };
            }
            return null;
        }
    }
}
