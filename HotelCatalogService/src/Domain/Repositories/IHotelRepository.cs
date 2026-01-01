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
            HotelStatus? status,
            Guid? ownerId,
            bool? isActive,
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
    }
}
