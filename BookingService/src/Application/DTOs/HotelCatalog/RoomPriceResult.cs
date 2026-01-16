namespace BookingService.Application.DTOs.HotelCatalog
{
    public class RoomPriceResult
    {
        public bool IsSuccess { get; set; }
        public decimal TotalPrice { get; set; } 
        public string Message { get; set; } = string.Empty;
        public Dictionary<DateOnly, decimal> DailyPrices { get; set; } = new();
    }
}
