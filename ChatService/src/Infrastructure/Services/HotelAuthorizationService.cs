using ChatService.Application.Common.Interfaces;
using ChatService.Domain.Common;

namespace ChatService.Infrastructure.Services
{
    public class HotelAuthorizationService : IHotelAuthorizationService
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IHotelCatalogService _hotelCatalogService;

        public HotelAuthorizationService(
            ICurrentUserService currentUser,
            IHotelCatalogService hotelCatalogService)
        {
            _currentUser = currentUser;
            _hotelCatalogService = hotelCatalogService;
        }

        public async Task<bool> HasHotelAccessAsync(Guid hotelId, CancellationToken cancellationToken = default)
        {
            if (_currentUser.Role == AppRoles.SysAdmin)
            {
                return true;
            }

            if (_currentUser.HotelId.HasValue && _currentUser.HotelId != Guid.Empty)
            {
                return _currentUser.HotelId == hotelId;
            }

            return await _hotelCatalogService.VerifyHotelOwnershipAsync(hotelId, cancellationToken);
        }
    }
}
