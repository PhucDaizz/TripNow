using HotelCatalogService.Application.Contracts;
using Microsoft.AspNetCore.Http;

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
            var response = await _httpClient.GetAsync($"/api/Booking/check-room-usage{RoomTypeId}", token);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}
