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

        public GetRevenueStatisticsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<RevenueDataPointDto>>> Handle(GetRevenueStatisticsQuery request, CancellationToken cancellationToken)
        {
            Guid? hotelId = request.HotelId;
            var role = _currentUserService.Role;

            if (role == AppRoles.HotelOwner || role == AppRoles.Receptionist)
            {
                hotelId = _currentUserService.HotelId;
            }

            var query = _context.Booking.AsNoTracking()
                .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                .Where(b => b.CreatedAt >= request.FromDate && b.CreatedAt <= request.ToDate);

            if (hotelId.HasValue)
            {
                query = query.Where(b => b.HotelId == hotelId.Value);
            }

            List<RevenueDataPointDto> result;

            if (request.GroupBy.Equals("Month", StringComparison.OrdinalIgnoreCase))
            {
                result = await query
                    .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                    .Select(g => new RevenueDataPointDto
                    {
                        Label = $"{g.Key.Month}/{g.Key.Year}",
                        Revenue = g.Sum(b => b.TotalAmount),
                        BookingCount = g.Count()
                    })
                    .ToListAsync(cancellationToken);
            }
            else
            {
                result = await query
                    .GroupBy(b => b.CreatedAt.Date)
                    .Select(g => new RevenueDataPointDto
                    {
                        Label = g.Key.ToString("yyyy-MM-dd"),
                        Revenue = g.Sum(b => b.TotalAmount),
                        BookingCount = g.Count()
                    })
                    .ToListAsync(cancellationToken);
            }

            return Result.Success(result);
        }
    }
}
