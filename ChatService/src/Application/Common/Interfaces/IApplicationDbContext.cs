using ChatService.Domain.Entities;

namespace ChatService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        IQueryable<Conversations> ConversationsQuery { get; }
        IQueryable<Messages> MessagesQuery { get; }
        IQueryable<ChatProfile> ChatProfilesQuery { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
