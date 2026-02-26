using BookingService.Domain.Enum;

namespace BookingService.Application.DTOs.Statistics
{
    public class HotelDashboardSummaryDto
    {
        public int TotalActiveBookings { get; set; }
        public int TodayCheckIns { get; set; }
        public int TodayCheckOuts { get; set; }
        public int PendingConfirmations { get; set; }
        public decimal TotalRevenueToDate { get; set; }
        public decimal ProjectedRevenue { get; set; }
        public List<RecentBookingDto> RecentBookings { get; set; } = new();
    }

    public class RecentBookingDto
    {
        public Guid BookingId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RevenueDataPointDto
    {
        public string Label { get; set; } // e.g., "2023-10-01" or "Oct 2023"
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
    }

    public class OccupancyDataPointDto
    {
        public string Label { get; set; }
        public double OccupancyRate { get; set; } // Percentage
        public int RoomsBooked { get; set; }
    }
}
