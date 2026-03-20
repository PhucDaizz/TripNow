namespace RecommendationService.Application.DTOs.Recommendation
{
    public class VectorSearchResult
    {
        public Guid HotelId { get; set; }
        public float Score { get; set; }
    }
}
