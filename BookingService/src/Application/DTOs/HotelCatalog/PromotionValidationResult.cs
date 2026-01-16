namespace BookingService.Application.DTOs.HotelCatalog
{
    public class PromotionValidationResult
    {
        public bool IsValid { get; set; }
        public decimal DiscountAmount { get; set; }
        public Guid PromotionId { get; set; }
        public string Message { get; set; }
    }
}
