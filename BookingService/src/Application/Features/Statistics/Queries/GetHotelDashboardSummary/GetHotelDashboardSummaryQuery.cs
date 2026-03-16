using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Statistics;
using BookingService.Domain.Enum;
using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Application.Features.Statistics.Queries.GetHotelDashboardSummary
{
    public class GetHotelDashboardSummaryQuery : IRequest<Result<HotelDashboardSummaryDto>>
    {
        public Guid? HotelId { get; set; } // Nếu Manager thì lấy từ Token, nếu Admin thì có thể truyền vào
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }

    public class GetHotelDashboardSummaryQueryHandler : IRequestHandler<GetHotelDashboardSummaryQuery, Result<HotelDashboardSummaryDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHotelAuthorizationService _hotelAuthService;

        public GetHotelDashboardSummaryQueryHandler(
            IApplicationDbContext context, 
            ICurrentUserService currentUserService,
            IHotelAuthorizationService hotelAuthService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _hotelAuthService = hotelAuthService;
        }

        public async Task<Result<HotelDashboardSummaryDto>> Handle(GetHotelDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            Guid? resolvedHotelId = (_currentUserService.HotelId.HasValue && _currentUserService.HotelId != Guid.Empty)
                ? _currentUserService.HotelId
                : request.HotelId;

            if (resolvedHotelId == null || resolvedHotelId == Guid.Empty)
            {
                return Result.Failure<HotelDashboardSummaryDto>(new Error("Statistics.HotelRequired", "Please provide the HotelId to view the Dashboard."));
            }

            Guid targetHotelId = resolvedHotelId.Value;

            bool hasAccess = await _hotelAuthService.HasHotelAccessAsync(targetHotelId, cancellationToken);

            if (!hasAccess)
            {
                return Result.Failure<HotelDashboardSummaryDto>(new Error("Auth.Forbidden", "You do not have permission to view the statistics of this hotel."));
            }

            var query = _context.Booking.AsNoTracking().Where(b => b.HotelId == targetHotelId);

            var today = request.Date;

            // Stats
            var stats = await query
                .GroupBy(b => 1) 
                .Select(g => new
                {
                    TotalActiveBookings = g.Sum(b => (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed) ? 1 : 0),
                    TodayCheckIns = g.Sum(b => (b.CheckInDate == today && b.Status == BookingStatus.Confirmed) ? 1 : 0),
                    TodayCheckOuts = g.Sum(b => (b.CheckOutDate == today && b.Status == BookingStatus.Confirmed) ? 1 : 0),
                    PendingConfirmations = g.Sum(b => b.Status == BookingStatus.Pending ? 1 : 0),

                    TotalRevenue = g.Sum(b => (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed) ? b.TotalAmount : 0),
                    ProjectedRevenue = g.Sum(b => b.Status == BookingStatus.Pending ? b.TotalAmount : 0)
                })
                .FirstOrDefaultAsync(cancellationToken);

            var recentBookings = await query
                .Where(b => b.Status != BookingStatus.Cancelled)
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .Select(b => new RecentBookingDto
                {
                    BookingId = b.Id,
                    CustomerName = b.CustomerName,
                    TotalAmount = b.TotalAmount,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var summary = new HotelDashboardSummaryDto
            {
                TotalActiveBookings = stats?.TotalActiveBookings ?? 0,
                TodayCheckIns = stats?.TodayCheckIns ?? 0,
                TodayCheckOuts = stats?.TodayCheckOuts ?? 0,
                PendingConfirmations = stats?.PendingConfirmations ?? 0,
                TotalRevenueToDate = stats?.TotalRevenue ?? 0,
                ProjectedRevenue = stats?.ProjectedRevenue ?? 0,
                RecentBookings = recentBookings
            };

            return Result.Success(summary);
        }
    }
}
