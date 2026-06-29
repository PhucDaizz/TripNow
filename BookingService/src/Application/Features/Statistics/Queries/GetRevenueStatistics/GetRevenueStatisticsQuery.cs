using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Statistics;
using BookingService.Domain.Common;
using BookingService.Domain.Enum;
using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Application.Features.Statistics.Queries.GetRevenueStatistics
{
    public class GetRevenueStatisticsQuery : IRequest<Result<List<RevenueDataPointDto>>>
    {
        public Guid? HotelId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string GroupBy { get; set; } = "Day"; // Day, Month
    }

    public class GetRevenueStatisticsQueryHandler : IRequestHandler<GetRevenueStatisticsQuery, Result<List<RevenueDataPointDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHotelAuthorizationService _hotelAuthService;

        public GetRevenueStatisticsQueryHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IHotelAuthorizationService hotelAuthService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _hotelAuthService = hotelAuthService;
        }

        public async Task<Result<List<RevenueDataPointDto>>> Handle(GetRevenueStatisticsQuery request, CancellationToken cancellationToken)
        {
            Guid? hotelId = request.HotelId;
            var role = _currentUserService.Role;

            if (role == AppRoles.HotelOwner || role == AppRoles.Receptionist)
            {
                hotelId = _currentUserService.HotelId;
            }

            if (hotelId.HasValue)
            {
                bool hasAccess = await _hotelAuthService.HasHotelAccessAsync(hotelId.Value, cancellationToken);
                if (!hasAccess)
                    return Result.Failure<List<RevenueDataPointDto>>(new Error("Auth.Forbidden", "You do not have permission to view the revenue statistics of this hotel."));
            }

            var toDateEndOfDay = request.ToDate.Date.AddDays(1).AddTicks(-1);

            var query = _context.Bookings.AsNoTracking()
                .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                .Where(b => b.CreatedAt >= request.FromDate && b.CreatedAt <= toDateEndOfDay);

            if (hotelId.HasValue)
            {
                query = query.Where(b => b.HotelId == hotelId.Value);
            }

            List<RevenueDataPointDto> result;

            if (request.GroupBy.Equals("Month", StringComparison.OrdinalIgnoreCase))
            {
                var monthlyRaw = await query
                    .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        Revenue = g.Sum(b => b.TotalAmount),
                        BookingCount = g.Count()
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToListAsync(cancellationToken);

                result = monthlyRaw.Select(x => new RevenueDataPointDto
                {
                    Label = $"{x.Month:D2}/{x.Year}",
                    Revenue = x.Revenue,
                    BookingCount = x.BookingCount
                }).ToList();
            }
            else
            {
                var dailyRaw = await query
                    .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month, b.CreatedAt.Day })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        g.Key.Day,
                        Revenue = g.Sum(b => b.TotalAmount),
                        BookingCount = g.Count()
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day)
                    .ToListAsync(cancellationToken);

                result = dailyRaw.Select(x => new RevenueDataPointDto
                {
                    Label = $"{x.Year}-{x.Month:D2}-{x.Day:D2}",
                    Revenue = x.Revenue,
                    BookingCount = x.BookingCount
                }).ToList();
            }

            return Result.Success(result);
        }
    }
}
