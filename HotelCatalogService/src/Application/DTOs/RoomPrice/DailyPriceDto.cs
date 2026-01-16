namespace HotelCatalogService.Application.DTOs.RoomPrice
{
    public class DailyPriceDto
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public bool IsSpecialPrice { get; set; }
    }
}
