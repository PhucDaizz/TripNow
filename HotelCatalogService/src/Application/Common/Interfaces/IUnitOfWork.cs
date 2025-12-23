using HotelCatalogService.Domain.Repositories;

namespace HotelCatalogService.Application.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        IHotelRepository Hotel { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
