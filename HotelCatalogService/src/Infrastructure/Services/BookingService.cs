using HotelCatalogService.Application.Contracts;
using HotelCatalogService.Application.DTOs.StaffProfile;
using Microsoft.AspNetCore.Http;
using Nexus.BuildingBlocks.Model;
using System.Net.Http.Json;

namespace HotelCatalogService.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookingService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CheckIsHaveAnyBookInFunitue(Guid RoomTypeId, CancellationToken token = default)
        {
            var response = await _httpClient.GetAsync("/api/Auth/staff-profile", token);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}
