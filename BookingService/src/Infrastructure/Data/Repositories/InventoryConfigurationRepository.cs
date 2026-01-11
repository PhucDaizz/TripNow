using BookingService.Domain.Entities;
using BookingService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Data.Repositories
{
    public class InventoryConfigurationRepository : IInventoryConfigurationRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryConfigurationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(InventoryConfiguration inventory, CancellationToken token = default)
        {
            await _context.InventoryConfiguration.AddAsync(inventory, token);
        }

        public async Task<bool> AnyAsync(Guid roomTypeId)
        {
            var isExisting = await _context.InventoryConfiguration.AnyAsync(x => x.RoomTypeId == roomTypeId);
            return isExisting;
        }

        public async Task<List<InventoryConfiguration>?> GetByHotelIdAsync(Guid hotelId, CancellationToken token = default)
        {
            var inventoryConfiguration = await _context.InventoryConfiguration.Where(x => x.HotelId == hotelId).ToListAsync(token);
            return inventoryConfiguration;
        }

        public async Task<InventoryConfiguration?> GetByRoomTypeIdAsync(Guid roomTypeId, CancellationToken token = default)
        {
            var inventoryConfiguration = await _context.InventoryConfiguration
                .FirstOrDefaultAsync(x => x.RoomTypeId == roomTypeId, token);
            return inventoryConfiguration;
        }
    }
}
