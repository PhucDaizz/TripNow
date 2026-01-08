using BookingService.Domain.Entities;

namespace BookingService.Domain.Repositories
{
    public interface IInventoryRepository
    {
        Task<Inventory?> GetInventoryByRoomTypeAndDateAsync(Guid roomTypeId, DateOnly date, CancellationToken cancellationToken = default);
        Task AddInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default);
        Task UpdateInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default);
        Task DeleteInventoryAsync(Inventory inventory);
        Task UpdateTotalStockBulkAsync(Guid roomTypeId, DateOnly fromDate, int quantityChange, CancellationToken cancellationToken);
        Task<List<DateOnly>> GetExistingDatesAsync(Guid roomTypeId, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken);
        Task AddRangeAsync(IEnumerable<Inventory> inventories, CancellationToken cancellationToken);
        Task UpdateTotalStockForDatesAsync(Guid roomTypeId, IEnumerable<DateOnly> dates, int quantityChange, CancellationToken cancellationToken);
        Task UpdateBlockedStockBulkAsync(Guid roomTypeId, IEnumerable<DateOnly> dates, int quantityChange, CancellationToken cancellationToken);
    }
}
