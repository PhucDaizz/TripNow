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
            // 1. Nếu không có Context Khách sạn -> Chắc chắn là Cá nhân
            if (!requestedHotelId.HasValue)
            {
                return AuthorType.User;
            }

            // 2. FAST-PATH: Dành cho nhân viên cấp dưới (Lễ tân, Quản lý sảnh...)
            // Token của họ được gắn chặt với 1 HotelId cố định lúc đăng nhập
            if (_currentUserService.HotelId.HasValue && _currentUserService.HotelId.Value == requestedHotelId.Value)
            {
                return AuthorType.Hotel;
            }

            // 3. SLOW-PATH: Dành cho Chủ Khách Sạn (Người có thể sở hữu nhiều KS)
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var role = _currentUserService.Role;

            if (role == AppRoles.HotelOwner)
            {
                // Phải check chéo sang Catalog xem cái Hotel người này đang muốn thao tác 
                // có thực sự do họ làm chủ (OwnerId) hay không.
                var hotelDetail = await _hotelCatalogService.GetHotelDetail(requestedHotelId.Value, cancellationToken);

                if (hotelDetail != null && hotelDetail.OwnerId == currentUserId)
                {
                    return AuthorType.Hotel;
                }
            }

            // 4. Các trường hợp còn lại (Ví dụ: Cố tình truyền láo HotelId của người khác)
            // thì tước quyền, ép về danh nghĩa User bình thường
            return AuthorType.User;
        }
    }
}
