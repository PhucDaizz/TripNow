using ChatService.Domain.Entities;

namespace ChatService.Domain.Repositories
{
    public interface IConversationRepository
    {
        Task AddAsync(Conversations conversation, CancellationToken token = default);
        Task<Conversations?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task DeleteAsync(Conversations conversation, CancellationToken token = default);
        Task<Conversations?> GetByUserIdAndHotelIdAsync(Guid userId, Guid hotelId, CancellationToken token = default);
        Task<List<Messages?>> GetUnreadMessage(Guid userReadId, Guid conversationId, CancellationToken token = default);
        Task<List<Conversations?>> GetAllConversation(Guid userId, CancellationToken token = default);

        Task<(List<Conversations> Items, int TotalCount)> GetConversationsByUserIdAsync(
            Guid userId,
            int pageIndex = 1,
            int pageSize = 20,
            CancellationToken token = default);

        Task<(List<Conversations> Items, int TotalCount)> GetConversationsByHotelIdAsync(
            Guid hotelId,
            int pageIndex = 1,
            int pageSize = 20,
            CancellationToken token = default);
    }
}
