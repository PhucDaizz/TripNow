using BookingService.Domain.Entities;
using BookingService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Data.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default)
        {
            await _context.Inventory.AddAsync(inventory, cancellationToken);
        }

        public Task DeleteInventoryAsync(Inventory inventory)
        {
            _context.Inventory.Remove(inventory);
            return Task.CompletedTask;
        }

        public async Task<Inventory?> GetInventoryByRoomTypeAndDateAsync(Guid roomTypeId, DateOnly date, CancellationToken cancellationToken = default)
        {
            return await _context.Inventory
                .FirstOrDefaultAsync(x => x.RoomTypeId == roomTypeId && x.Date == date, cancellationToken);
        }

        public async Task UpdateTotalStockBulkAsync(Guid roomTypeId, DateOnly fromDate, int quantityChange, CancellationToken cancellationToken)
        {
            await _context.Inventory 
                .Where(x => x.RoomTypeId == roomTypeId && x.Date >= fromDate)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    i => i.TotalStock,
                    i => i.TotalStock + quantityChange), 
                cancellationToken);
        }

        public Task UpdateInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default)
        {
            _context.Inventory.Update(inventory);
            return Task.CompletedTask;
        }

        public async Task<List<DateOnly>> GetExistingDatesAsync(Guid roomTypeId, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken)
        {
            return await _context.Inventory
                .Where(x => x.RoomTypeId == roomTypeId && x.Date >= fromDate && x.Date <= toDate)
                .Select(x => x.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<Inventory> inventories, CancellationToken cancellationToken)
        {
            await _context.Inventory.AddRangeAsync(inventories, cancellationToken);
        }

        public async Task UpdateTotalStockForDatesAsync(Guid roomTypeId, IEnumerable<DateOnly> dates, int quantityChange, CancellationToken cancellationToken)
        {
            if (dates == null || !dates.Any()) return;

            await _context.Inventory
                .Where(x => x.RoomTypeId == roomTypeId && dates.Contains(x.Date)) 
                .ExecuteUpdateAsync(s => s.SetProperty(
                    i => i.TotalStock,
                    i => i.TotalStock + quantityChange), 
                cancellationToken);
        }

        public async Task UpdateBlockedStockBulkAsync(Guid roomTypeId, IEnumerable<DateOnly> dates, int quantityChange, CancellationToken cancellationToken)
        {
            if (dates == null || !dates.Any()) return;

            await _context.Inventory
                .Where(x => x.RoomTypeId == roomTypeId && dates.Contains(x.Date)) 
                .ExecuteUpdateAsync(s => s.SetProperty(
                    i => i.BlockedStock,
                    i => i.BlockedStock + quantityChange), 
                cancellationToken);
        }

        public async Task<List<DateOnly>> GetDatesInRangeAsync(Guid roomTypeId, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken)
        {
            return await _context.Inventory
                .AsNoTracking() 
                .Where(x => x.RoomTypeId == roomTypeId
                            && x.Date >= fromDate
                            && x.Date <= toDate)
                .Select(x => x.Date) 
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Inventory>?> GetInventoriesInRangeAsync(List<Guid>roomTypeIds, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default)
        {
            return await _context.Inventory
                .Where(x => roomTypeIds.Contains(x.RoomTypeId)
                     && x.Date >= fromDate
                     && x.Date < toDate)
                .ToListAsync(cancellationToken);
        }

        public async Task SetBlockedStockToZeroBulkAsync(Guid roomTypeId, IEnumerable<DateOnly> dates, CancellationToken cancellationToken)
        {
            if (dates == null || !dates.Any()) return;

            await _context.Inventory
                .Where(x => x.RoomTypeId == roomTypeId && dates.Contains(x.Date))
                .ExecuteUpdateAsync(s => s.SetProperty(
                    i => i.BlockedStock,
                    i => 0), // Set cứng về 0
                cancellationToken);
        }
    }
}
