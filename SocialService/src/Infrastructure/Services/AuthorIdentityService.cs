using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;
using SocialService.Domain.Common;
using SocialService.Domain.Enum;

namespace SocialService.Infrastructure.Services
{
    public class AuthorIdentityService : IAuthorIdentityService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IHotelCatalogService _hotelCatalogService;

        public AuthorIdentityService(ICurrentUserService currentUserService, IHotelCatalogService hotelCatalogService)
        {
            _currentUserService = currentUserService;
            _hotelCatalogService = hotelCatalogService;
        }

        public async Task<AuthorType> ResolveAuthorTypeAsync(Guid? requestedHotelId, CancellationToken cancellationToken = default)
        {
            if (!requestedHotelId.HasValue)
            {
                return AuthorType.User;
            }

            if (_currentUserService.HotelId.HasValue && _currentUserService.HotelId.Value == requestedHotelId.Value)
            {
                return AuthorType.Hotel;
            }

            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var role = _currentUserService.Role;

            if (role == AppRoles.HotelOwner)
            {
                var hotelDetail = await _hotelCatalogService.GetHotelDetail(requestedHotelId.Value, cancellationToken);

                if (hotelDetail != null && hotelDetail.OwnerId == currentUserId)
                {
                    return AuthorType.Hotel;
                }
            }

            return AuthorType.User;
        }
    }
}
