using BookingService.Application.DTOs.Inventory;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.Booking.Queries.GetAvailability
{
    public class GetAvailabilityQuery : IRequest<Result<List<DailyAvailabilityDto>>>
    {
        public Guid RoomTypeId { get; set; }

        // Option A: Xem theo tháng (Calendar View)
        public int? Month { get; set; }
        public int? Year { get; set; }

        // Option B: Check khoảng ngày cụ thể (Range Search)
        public DateOnly? CheckInDate { get; set; }
        public DateOnly? CheckOutDate { get; set; }
    }
}
