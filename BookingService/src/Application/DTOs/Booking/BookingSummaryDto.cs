namespace BookingService.Application.DTOs.Booking
{
    public class BookingSummaryDto
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public string? HotelName { get; set; } // Cần join hoặc map từ cached
        public Guid UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; } 
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
