namespace BookingService.Application.DTOs.HotelCatalog
{
    public class CatalogBatchPriceDto
    {
        public Guid RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public decimal BasePrice { get; set; }
        public List<CatalogDailyPriceDto> Calendar { get; set; } = new();
        public CancellationPolicyDto? CancellationPolicy { get; set; }
    }

    public class CatalogDailyPriceDto
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public bool IsSpecialPrice { get; set; }
    }

    public class CancellationPolicyDto
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<CancellationRuleDto> Rules { get; set; } = new();
    }

    public class CancellationRuleDto
    {
        public Guid Id { get; set; }
        public int HoursBeforeCheckIn { get; set; }
        public decimal RefundPercentage { get; set; }
    }
}
