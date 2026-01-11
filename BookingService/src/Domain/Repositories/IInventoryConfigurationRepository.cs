using BookingService.Domain.Entities;

namespace BookingService.Domain.Repositories
{
    public interface IInventoryConfigurationRepository
    {
        Task<InventoryConfiguration?> GetByRoomTypeIdAsync(Guid roomTypeId, CancellationToken token = default);
        Task<List<InventoryConfiguration>> GetByHotelIdAsync(Guid hotelId, CancellationToken token = default);
        Task<bool> AnyAsync(Guid roomTypeId);
        Task AddAsync(InventoryConfiguration inventory, CancellationToken token = default);
    }
}
