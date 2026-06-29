using BookingService.Domain.Entities;

namespace BookingService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public IQueryable<Booking> Bookings { get; }
        public IQueryable<BookingCancellation> BookingCancellations { get; }
        public IQueryable<BookingItem> BookingItems { get; }
        public IQueryable<BookingPriceDetail> BookingPriceDetails { get; }
        public IQueryable<BookingPriceSnapshot> BookingPriceSnapshots { get; }
        public IQueryable<Inventory> Inventories { get; }
        public IQueryable<RoomAssignment> RoomAssignments { get; }
        public IQueryable<InventoryConfiguration> InventoryConfigurations { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
