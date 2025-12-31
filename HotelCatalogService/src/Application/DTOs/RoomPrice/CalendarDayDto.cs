namespace HotelCatalogService.Application.DTOs.RoomPrice
{
    public class CalendarDayDto
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public bool IsSpecialPrice { get; set; } 
        // public bool IsSoldOut { get; set; }
    }
}
