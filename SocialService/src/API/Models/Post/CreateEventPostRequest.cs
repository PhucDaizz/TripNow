namespace SocialService.API.Models.Post
{
    public class CreateEventPostRequest
    {
        public Guid HotelId { get; set; }
        public string Content { get; set; }
        public List<IFormFile>? Images { get; set; }
        public List<string>? ImageCaptions { get; set; }
    }
}
