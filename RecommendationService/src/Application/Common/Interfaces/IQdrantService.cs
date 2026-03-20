using RecommendationService.Application.DTOs.Recommendation;

namespace RecommendationService.Application.Common.Interfaces
{
    public interface IQdrantService
    {
        Task EnsureCollectionExistsAsync(string collectionName, ulong vectorSize);
        Task UpsertVectorAsync(string collectionName, Guid id, float[] vector, Dictionary<string, string>? payload = null);
        Task<IReadOnlyList<VectorSearchResult>> SearchSimilarAsync(string collectionName, float[] queryVector, ulong limit = 10);
        Task<IReadOnlyList<VectorSearchResult>> RecommendAsync(string collectionName, IEnumerable<Guid> positiveHotelIds, ulong limit = 10);
    }
}
