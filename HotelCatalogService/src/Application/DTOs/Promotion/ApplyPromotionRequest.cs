namespace HotelCatalogService.Application.DTOs.Promotion
{
    public class ApplyPromotionRequest
    {
        public string Code { get; set; }
        public Guid UserId { get; set; }
        public Guid BookingId { get; set; }
        public decimal OrderAmount { get; set; }
    }
}
