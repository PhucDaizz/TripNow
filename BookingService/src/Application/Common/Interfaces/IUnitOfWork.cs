using BookingService.Domain.Repositories;

namespace BookingService.Application.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBookingRepository Booking { get; }
        IInventoryRepository Inventory { get; }
        IInventoryConfigurationRepository InventoryConfiguration { get; }
        IBookingPriceSnapshotRepository BookingPriceSnapshot { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
