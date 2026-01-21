namespace BookingService.Application.DTOs.Booking
{
    public class CancelUnpaidBookingDto
    {
        public Guid BookingId { get; set; }
        public string Reason { get; set; }
    }
}
