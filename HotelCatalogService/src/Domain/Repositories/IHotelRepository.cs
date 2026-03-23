using HotelCatalogService.Domain.Common.Models;
using HotelCatalogService.Domain.Dto.Hotel;
using HotelCatalogService.Domain.Dto.Room;
using HotelCatalogService.Domain.Entities;
using HotelCatalogService.Domain.Enum;
using System.Linq.Expressions;

namespace HotelCatalogService.Domain.Repositories
{
    public interface IHotelRepository
    {
        Task AddAsync(Hotel hotel, CancellationToken cancellationToken = default);
        Task UpdateAsync(Hotel hotel, CancellationToken cancellationToken = default);

        Task DeleteAsync(Hotel hotel, CancellationToken cancellationToken = default);

        Task<Hotel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Hotel?> GetByIdIncludeAsync(
            Guid id,
            CancellationToken cancellationToken = default,
            params Expression<Func<Hotel, object>>[] includes
        );

        Task<Hotel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Hotel>> GetByOwnerIdAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<HotelDto>> GetByFilterAsync(
            string? searchTerm,
            string? city,
            HotelStatus? status,
            Guid? ownerId,
            decimal? minRating,
            double? userLat, double? userLon, double? radiusKm,
            decimal? minPrice, decimal? maxPrice, 
            string? sortColumn, string? sortDirection, 
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Hotel?> GetHotelWithRoomTypesAndImagesAsync(Guid hotelId, CancellationToken cancellationToken = default);
        Task<Hotel?> GetHotelWithBlocksAndFloorsAsync(Guid hotelId, CancellationToken cancellationToken = default);
        Task<Hotel?> GetHotelForRoomSetupAsync(Guid hotelId, Guid blockId, Guid floorId, CancellationToken token = default);
        Task<List<DirtyRoomDto>> GetDirtyRoomsAsync(Guid hotelId, Guid? blockId, Guid? floorId, CancellationToken token);
        Task<Hotel?> GetHotelWithSpecificRoomAsync(Guid hotelId, Guid roomId, CancellationToken token = default);
        Task<Hotel?> GetHotelWithRoomTypePricesAsync(Guid hotelId, Guid roomTypeId, CancellationToken token);
        Task<Hotel?> GetHotelCatalogAsync(Guid hotelId, DateTime checkInDate, CancellationToken token);
        Task<Hotel?> GetHotelWithPromotionsAsync(Guid hotelId, CancellationToken token);
        Task<Hotel?> GetHotelWithSpecificPromotionAsync(Guid hotelId, string code, CancellationToken token);
        Task<bool> IsPromotionUsedByUserAsync(Guid hotelId, string code, Guid userId, CancellationToken token);
        Task<Hotel?> GetHotelWithBookingPromotionUsageAsync(Guid hotelId, string code, Guid bookingId, CancellationToken token);
        Task<bool> HasRoomsInFloorAsync(Guid floorId, CancellationToken cancellationToken = default);
        Task<Hotel?> GetHotelWithFullStructureAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
