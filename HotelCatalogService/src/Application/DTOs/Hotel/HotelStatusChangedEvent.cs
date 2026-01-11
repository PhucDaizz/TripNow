namespace HotelCatalogService.Application.DTOs.Hotel
{
    public class HotelStatusChangedEvent
    {
        public Guid HotelId { get; set; }
        public bool IsClosed { get; set; } // True = Đóng, False = Mở
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
