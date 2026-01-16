namespace BookingService.Application.DTOs.HotelCatalog
{
    public class CheckPromotionRequestDto
    {
        public string Code { get; set; }
        public decimal OrderAmount { get; set; }
        public Guid? UserId { get; set; }
    }
}
