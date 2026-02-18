namespace SocialService.Application.DTOs.PostImage
{
    public class PostImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public int SortOrder { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Caption { get; set; }
    }
}
