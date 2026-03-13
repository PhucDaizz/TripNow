using ChatService.Domain.Repositories;

namespace ChatService.Application.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IConversationRepository Conversation { get;}
        IChatProfileRepository ChatProfile { get;}
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
