namespace BookingService.Application.DTOs.HotelCatalog
{
    public class CatalogBatchPriceDto
    {
        public Guid RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public decimal BasePrice { get; set; }
        public List<CatalogDailyPriceDto> Calendar { get; set; } = new();
    }

    public class CatalogDailyPriceDto
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public bool IsSpecialPrice { get; set; }
    }
}
