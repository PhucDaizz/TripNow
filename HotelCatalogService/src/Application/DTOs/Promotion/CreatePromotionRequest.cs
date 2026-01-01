using HotelCatalogService.Domain.Enum;

namespace HotelCatalogService.Application.DTOs.Promotion
{
    public class CreatePromotionRequest
    {
        public string Code { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
    }
}
