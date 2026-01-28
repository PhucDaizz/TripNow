namespace BookingService.Application.DTOs.BookingPriceDetail
{
    public class BookingPriceDetailDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } 
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
