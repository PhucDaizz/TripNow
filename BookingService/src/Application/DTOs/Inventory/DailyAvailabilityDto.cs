namespace BookingService.Application.DTOs.Inventory
{
    public class DailyAvailabilityDto
    {
        public DateOnly Date { get; set; }
        public int TotalStock { get; set; }
        public int SoldStock { get; set; }
        public int BlockedStock { get; set; }
        public int AvailableStock => TotalStock - SoldStock - BlockedStock;
        public bool IsSoldOut => AvailableStock <= 0;
    }
}
