using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace HotelCatalogService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public IHotelRepository Hotel { get; }
        public IAmenityRepository Amenity { get; }
        public ICancellationPolicyRepository CancellationPolicy { get; }

        public UnitOfWork(ApplicationDbContext context,
            IHotelRepository hotelRepository,
            IAmenityRepository amenityRepository,
            ICancellationPolicyRepository cancellationPolicyRepository)
        {
            _context = context;
            Hotel = hotelRepository;
            Amenity = amenityRepository;
            CancellationPolicy = cancellationPolicyRepository;
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
