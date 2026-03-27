using RecommendationService.Application.DTOs.Recommendation;

namespace RecommendationService.Application.Common.Interfaces
{
    public interface IQdrantService
    {
        Task EnsureCollectionExistsAsync(string collectionName, ulong vectorSize);
        Task UpsertVectorAsync(string collectionName, Guid id, float[] vector, Dictionary<string, object>? payload = null);
        Task<IReadOnlyList<VectorSearchResult>> SearchSimilarAsync(string collectionName, float[] queryVector, ulong limit = 10);
        Task<IReadOnlyList<VectorSearchResult>> RecommendAsync(
            string collectionName, 
            IEnumerable<Guid> positiveHotelIds,
            List<string>? preferredCities = null,
            ulong limit = 10);

        Task<List<string>> GetCitiesByHotelIdsAsync(string collectionName, IEnumerable<Guid> hotelIds);
        Task<IReadOnlyList<VectorSearchResult>> GetSimilarHotelsAdvancedAsync(string collectionName, Guid currentHotelId, string targetCity, ulong totalLimit = 5);
        Task<IEnumerable<Guid>> GetRandomHotelsAsync(string collectionName, ulong limit);
    }
}
