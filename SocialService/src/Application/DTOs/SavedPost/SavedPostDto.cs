namespace SocialService.Application.DTOs.SavedPost
{
    public class SavedPostDto
    {
        public Guid PostId { get; set; }
        public string ContentShort { get; set; } 
        public string ThumbnailUrl { get; set; }
        public DateTime SavedAt { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}
