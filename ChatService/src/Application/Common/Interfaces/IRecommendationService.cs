namespace ChatService.Application.Common.Interfaces
{
    public interface IRecommendationService
    {
        /// <summary>
        /// Giao tiếp với Recommendation Service để lấy Ngữ cảnh (RAG Context) từ Qdrant
        /// </summary>
        Task<List<string>> GetHotelChatContextAsync(Guid hotelId, string userMessage, int limit = 3, CancellationToken cancellationToken = default);
    }
}
