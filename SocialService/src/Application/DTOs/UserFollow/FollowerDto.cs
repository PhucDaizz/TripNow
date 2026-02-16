namespace SocialService.Application.DTOs.UserFollow
{
    public class FollowerDto
    {
        public Guid FollowerId { get; set; } 
        public string FollowerName { get; set; }
        public string FollowerAvatar { get; set; }
        public DateTime FollowedAt { get; set; }
    }
}
