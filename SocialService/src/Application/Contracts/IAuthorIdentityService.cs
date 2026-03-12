using SocialService.Domain.Enum;

namespace SocialService.Application.Contracts
{
    public interface IAuthorIdentityService
    {
        /// <summary>
        /// Xác định xem người dùng hiện tại đang thao tác với tư cách Khách cá nhân hay Khách sạn
        /// </summary>
        Task<PostAuthorType> ResolveAuthorTypeAsync(Guid? requestedHotelId, CancellationToken cancellationToken = default);
    }
}
