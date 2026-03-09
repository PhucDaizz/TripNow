using NotificationService.Domain.Repositories;

namespace NotificationService.Application.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        INotificationRepository Notification { get; }
        ISocialNotificationRepository SocialNotification { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
