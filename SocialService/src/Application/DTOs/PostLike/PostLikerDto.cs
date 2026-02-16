namespace SocialService.Application.DTOs.PostLike
{
    public class PostLikerDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime LikedAt { get; set; }
    }
}
