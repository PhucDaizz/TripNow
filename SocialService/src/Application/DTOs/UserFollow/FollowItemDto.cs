namespace SocialService.Application.DTOs.UserFollow
{
    public class FollowItemDto
    {
        public Guid TargetId { get; set; }
        public string TargetName { get; set; } 
        public string TargetAvatar { get; set; } 
        public string Type { get; set; } 
        public DateTime FollowedAt { get; set; }
    }
}
