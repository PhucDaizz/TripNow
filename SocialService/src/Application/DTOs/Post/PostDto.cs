using SocialService.Application.DTOs.Review;

namespace SocialService.Application.DTOs.Post
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public string Content { get; set; }
        public string Type { get; set; } // "Normal", "Event", "Review"

        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }

        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; } 

        public List<PostImageDto> Images { get; set; } = new();

        public ReviewDetailDto? Review { get; set; }
    }
}
