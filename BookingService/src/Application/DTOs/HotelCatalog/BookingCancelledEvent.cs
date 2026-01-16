namespace BookingService.Application.DTOs.HotelCatalog
{
    public record BookingCancelledEvent
    {
        public Guid HotelId { get; set; }
        public Guid BookingId { get; set; }
        public string PromotionCode { get; set; }

    }
}
