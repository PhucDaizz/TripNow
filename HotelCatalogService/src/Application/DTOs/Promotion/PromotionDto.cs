namespace HotelCatalogService.Application.DTOs.Promotion
{
    public class PromotionDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; } // "Percent" / "Amount"
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int InitialQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public int UsedQuantity => InitialQuantity - RemainingQuantity; 

        public bool IsActive { get; set; } // Config Setting
        public string Status { get; set; } // Display Text
    }
}
