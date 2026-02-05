namespace BookingService.Application.DTOs.Booking
{
    public class BookingCompletedIntegrationEvent
    {
        public Guid BookingId { get; set; }
        public Guid HotelId { get; set; }
        public decimal Amount { get; set; }
    }
}
