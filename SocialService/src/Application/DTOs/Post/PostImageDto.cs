namespace SocialService.Application.DTOs.Post
{
    public class PostImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public int SortOrder { get; set; }
        public string Caption { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
