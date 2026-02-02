using BookingService.Application.Common.Interfaces;
using BookingService.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookingService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public IBookingRepository Booking { get; }
        public IInventoryRepository Inventory { get; }
        public IInventoryConfigurationRepository InventoryConfiguration { get; }
        public IBookingPriceSnapshotRepository BookingPriceSnapshot { get; }


        public UnitOfWork(ApplicationDbContext context,
            IBookingRepository bookingRepository,
            IInventoryRepository inventoryRepository,
            IInventoryConfigurationRepository inventoryConfigurationRepository,
            IBookingPriceSnapshotRepository bookingPriceSnapshotRepository)
        {
            _context = context;
            Booking = bookingRepository;
            Inventory = inventoryRepository;
            InventoryConfiguration = inventoryConfigurationRepository;
            BookingPriceSnapshot = bookingPriceSnapshotRepository;
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
