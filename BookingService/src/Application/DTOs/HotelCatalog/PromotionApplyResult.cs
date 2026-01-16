namespace BookingService.Application.DTOs.HotelCatalog
{
    public class PromotionApplyResult
    {
        public bool IsSuccess { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Message { get; set; }
    }
}
