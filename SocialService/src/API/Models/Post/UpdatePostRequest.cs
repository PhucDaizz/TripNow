namespace SocialService.API.Models.Post
{
    public class UpdatePostRequest
    {
        public string Content { get; set; }

        public List<Guid>? DeletedImageIds { get; set; } 

        public List<IFormFile>? NewImages { get; set; } 
        public List<string>? NewImageCaptions { get; set; } 
    }
}
