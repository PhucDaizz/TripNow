using SocialService.Domain.Entities;

namespace SocialService.Domain.Repositories
{
    public interface IMemberRepository
    {
        Task<Member?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task<bool> IsExistingAsync(Guid id, CancellationToken token = default);
        Task AddAsync(Member member, CancellationToken token = default);
        Task UpdateAsync(Member member, CancellationToken token = default);
        Task DeleteAsync(Member member, CancellationToken token = default);
    }
}
