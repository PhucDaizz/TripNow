namespace HotelCatalogService.Application.DTOs.Promotion
{
    public class PromotionDiscountDto
    {
        public Guid PromotionId { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal FinalDiscountAmount { get; set; } 
    }
}
