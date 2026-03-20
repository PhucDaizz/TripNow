namespace RecommendationService.Application.DTOs.UserSearchHistory
{
    public class UserSearchHistoryDto
    {
        public Guid UserId { get; set; }
        public string RawQuery { get; set; } 
        public string? Destination { get; set; } 
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public DateTime SearchedAt { get; set; }
    }
}
