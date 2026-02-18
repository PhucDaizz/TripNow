namespace SocialService.Application.DTOs.Review
{
    public class ReviewDetailDto
    {
        public Guid TargetId { get; set; }
        public string TargetType { get; set; }
        public decimal Rating { get; set; }
        public Guid? BookingId { get; set; }
    }
}
