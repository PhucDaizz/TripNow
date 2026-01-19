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

        /// <summary>
        /// Lấy danh sách các ngày ĐÃ CÓ Inventory trong một khoảng thời gian (để biết ngày nào cần reset).
        /// </summary>
        Task<List<DateOnly>> GetDatesInRangeAsync(Guid roomTypeId, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken);
        /// <summary>
        /// Lấy danh sách Inventory trong một khoảng thời gian.     
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<Inventory>?> GetInventoriesInRangeAsync(List<Guid> roomTypeIds, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken);

        /// <summary>
        /// Reset cột BlockedStock về 0 hàng loạt (Dùng khi mở cửa khách sạn lại).
        /// </summary>
        Task SetBlockedStockToZeroBulkAsync(Guid roomTypeId, IEnumerable<DateOnly> dates, CancellationToken cancellationToken);
    }
}
