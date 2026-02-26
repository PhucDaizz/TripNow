namespace BookingService.Application.DTOs.Booking.Event
{
    public class RestorePromotionDto
    {
        public Guid BookingId { get; set; }
        public Guid HotelId { get; set; }
        public string PromotionCode { get; set; }
    }
}
