using SocialService.Application.DTOs.Hotel;
using System.Net.Http;

namespace SocialService.Application.Contracts
{
    public interface IHotelCatalogService
    {
        Task<bool> IsHotelExisting(Guid hotelId, CancellationToken cancellationToken = default);
        Task<HotelDetailDto?> GetHotelDetail(Guid hotelId, CancellationToken cancellation = default);
    }
}