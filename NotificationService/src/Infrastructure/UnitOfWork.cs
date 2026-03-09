using Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Repositories;

namespace NotificationService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public INotificationRepository Notification { get; }
        public ISocialNotificationRepository SocialNotification { get; }

        public UnitOfWork(ApplicationDbContext context, INotificationRepository notificationRepository, ISocialNotificationRepository socialNotification)
        {
            _context = context;
            Notification = notificationRepository;
            SocialNotification = socialNotification;
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
