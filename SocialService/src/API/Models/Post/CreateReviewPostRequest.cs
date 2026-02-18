using SocialService.Domain.Enum;

namespace SocialService.API.Models.Post
{
    public class CreateReviewPostRequest
    {
        public Guid HotelId { get; set; }
        public string Content { get; set; }

        public TargetTypeReview TargetType { get; set; }
        public Guid TargetId { get; set; }
        public decimal Rating { get; set; }
        public Guid? BookingId { get; set; }

        public List<IFormFile>? Images { get; set; }
        public List<string>? ImageCaptions { get; set; }
    }
}
