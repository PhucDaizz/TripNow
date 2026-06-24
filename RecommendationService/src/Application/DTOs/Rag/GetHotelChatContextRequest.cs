namespace RecommendationService.Application.DTOs.Rag
{
    public class GetHotelChatContextRequest
    {
        public string Message { get; set; } = string.Empty;
        public int Limit { get; set; } = 3; 
    }
}
