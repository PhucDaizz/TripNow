using ChatService.Domain.Entities;

namespace ChatService.Domain.Repositories
{
    public interface IChatProfileRepository
    {
        Task<ChatProfile?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task<bool> IsExistingAsync(Guid id, CancellationToken token = default);
        Task AddAsync(ChatProfile chatProfile, CancellationToken token = default);
        Task UpdateAsync(ChatProfile chatProfile, CancellationToken token = default);
        Task DeleteAsync(ChatProfile chatProfile, CancellationToken token = default);
        Task<List<ChatProfile>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    }
}
