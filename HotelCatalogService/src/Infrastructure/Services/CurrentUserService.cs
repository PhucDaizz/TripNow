using HotelCatalogService.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HotelCatalogService.Infrastructure.Services
{
    public class CurrentUserService: ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).ToString();

        public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name).ToString();

        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email).ToString();

        public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role).ToString();

        public Guid? HotelId
        {
            get
            {
                var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("hotelId")?.Value
                                   ?? _httpContextAccessor.HttpContext?.User?.FindFirst("HotelId")?.Value;

                if (string.IsNullOrEmpty(hotelIdClaim))
                {
                    return null;
                }

                return Guid.TryParse(hotelIdClaim, out var hotelId) ? hotelId : null;
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
