using BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Booking> Booking { get; set; }
        public DbSet<BookingCancellation> BookingCancellation { get; set; }
        public DbSet<BookingItem> BookingItem { get; set; }
        public DbSet<BookingPriceDetail> BookingPriceDetail { get; set; }
        public DbSet<BookingPriceSnapshot> BookingPriceSnapshot { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<RoomAssignment> RoomAssignment { get; set; }
        public DbSet<InventoryConfiguration> InventoryConfiguration { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
