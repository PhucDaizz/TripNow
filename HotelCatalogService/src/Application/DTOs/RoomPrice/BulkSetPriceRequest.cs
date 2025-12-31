namespace HotelCatalogService.Application.DTOs.RoomPrice
{
    public class BulkSetPriceRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal Price { get; set; }
        public List<DayOfWeek>? SpecificDays { get; set; }
    }
}
