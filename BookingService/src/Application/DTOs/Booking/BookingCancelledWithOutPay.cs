namespace BookingService.Application.DTOs.Booking
{
    public class BookingCancelledWithOutPay
    {
        public Guid BookingId { get; set; }
        public string Reason { get; set; }
    }
}
