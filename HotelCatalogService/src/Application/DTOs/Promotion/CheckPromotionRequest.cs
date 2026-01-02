namespace HotelCatalogService.Application.DTOs.Promotion
{
    public class CheckPromotionRequest
    {
        public string Code { get; set; }
        public decimal OrderAmount { get; set; }
    }
}
