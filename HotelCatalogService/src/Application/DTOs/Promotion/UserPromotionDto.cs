namespace HotelCatalogService.Application.DTOs.Promotion
{
    public class UserPromotionDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string DiscountInfo { get; set; } 
        public DateTime EndDate { get; set; }

        public int? RemainingCount { get; set; } 
        public string TagLabel { get; set; }     
    }
}
