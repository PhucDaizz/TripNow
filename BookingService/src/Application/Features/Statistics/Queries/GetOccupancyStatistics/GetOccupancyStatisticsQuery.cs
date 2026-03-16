using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Statistics;
using BookingService.Domain.Common;
using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Application.Features.Statistics.Queries.GetOccupancyStatistics
{
    public class GetOccupancyStatisticsQuery : IRequest<Result<List<OccupancyDataPointDto>>>
    {
        public Guid? HotelId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
    }

    public class GetOccupancyStatisticsQueryHandler : IRequestHandler<GetOccupancyStatisticsQuery, Result<List<OccupancyDataPointDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHotelAuthorizationService _hotelAuthService;

        public GetOccupancyStatisticsQueryHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IHotelAuthorizationService hotelAuthService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _hotelAuthService = hotelAuthService;
        }

        public async Task<Result<List<OccupancyDataPointDto>>> Handle(GetOccupancyStatisticsQuery request, CancellationToken cancellationToken)
        {
            Guid? targetHotelId = (_currentUserService.HotelId.HasValue && _currentUserService.HotelId != Guid.Empty)
                ? _currentUserService.HotelId
                : request.HotelId;

            if (targetHotelId == null || targetHotelId == Guid.Empty)
            {
                return Result.Failure<List<OccupancyDataPointDto>>(
                    new Error("Statistics.HotelRequired", "HotelId is required for occupancy statistics."));
            }

            bool hasAccess = await _hotelAuthService.HasHotelAccessAsync(targetHotelId.Value, cancellationToken);
            if (!hasAccess)
                return Result.Failure<List<OccupancyDataPointDto>>(
                    new Error("Auth.Forbidden", "You do not have permission to view the occupancy statistics of this hotel."));

            var roomTypeIds = await _context.Booking
                .Where(b => b.HotelId == targetHotelId.Value) 
                .SelectMany(b => b.Items)
                .Select(i => i.RoomTypeId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!roomTypeIds.Any())
            {
                return Result.Success(new List<OccupancyDataPointDto>());
            }

            var inventoryRaw = await _context.Inventory
                .AsNoTracking()
                .Where(i => roomTypeIds.Contains(i.RoomTypeId))
                .Where(i => i.Date >= request.FromDate && i.Date <= request.ToDate)
                .GroupBy(i => i.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    RoomsBooked = g.Sum(i => i.SoldStock),
                    TotalStock = g.Sum(i => i.TotalStock)
                })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            var inventoryData = inventoryRaw.Select(x => new OccupancyDataPointDto
            {
                Label = x.Date.ToString("yyyy-MM-dd"),
                RoomsBooked = x.RoomsBooked,
                OccupancyRate = x.TotalStock > 0
                    ? (double)x.RoomsBooked / x.TotalStock * 100
                    : 0
            }).ToList();

            return Result.Success(inventoryData);
        }
    }
}
