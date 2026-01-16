namespace HotelCatalogService.Application.DTOs.RoomPrice
{
    public class RoomTypeCalendarDto
    {
        public Guid RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public decimal BasePrice { get; set; }
        public List<DailyPriceDto> Calendar { get; set; } = new();
    }
}
