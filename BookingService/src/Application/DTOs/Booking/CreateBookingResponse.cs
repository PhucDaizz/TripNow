namespace BookingService.Application.DTOs.Booking
{
    public class CreateBookingResponse
    {
        public Guid BookingId { get; init; }
        public string PaymentUrl { get; init; }
    }
}
