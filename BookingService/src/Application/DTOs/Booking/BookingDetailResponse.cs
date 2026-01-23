namespace BookingService.Application.DTOs.Booking
{
    public class BookingDetailResponse
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public Guid UserId { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<BookingItemDetailDTO> Items { get; set; } = new();

    }

    public class BookingItemDetailDTO
    {
        public Guid RoomTypeId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
