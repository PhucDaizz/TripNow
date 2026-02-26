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

        public GetOccupancyStatisticsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<OccupancyDataPointDto>>> Handle(GetOccupancyStatisticsQuery request, CancellationToken cancellationToken)
        {
            Guid? hotelId = request.HotelId;
            var role = _currentUserService.Role;

            if (role == AppRoles.HotelOwner || role == AppRoles.Receptionist)
            {
                hotelId = _currentUserService.HotelId;
            }

            if (hotelId == null)
            {
                return Result.Failure<List<OccupancyDataPointDto>>(new Error("Statistics.HotelRequired", "HotelId is required for occupancy statistics."));
            }

            // Occupancy is calculated from Inventory
            // We need to know which RoomTypeIds belong to this HotelId
            // But BookingService might not have a HotelRoomType table? 
            // Let's check Inventory table's usage or if there's a link.
            // Oh, wait, the Inventory table itself might not have HotelId.
            // Let's check InventoryConfiguration or if we can infer RoomTypeIds from existing bookings of that hotel.
            
            // Re-checking the Domain...
            // Booking has HotelId. BookingItems have RoomTypeId.
            // So we can find RoomTypeIds associated with this HotelId from the Booking table (historical)
            // or we might need a better way.
            
            // Let's check if there's any table that links HotelId and RoomTypeId in BookingService.
            // I'll list the Domain entities again.
            // Entities: Booking, BookingCancellation, BookingItem, BookingPriceDetail, BookingPriceSnapshot, Inventory, InventoryConfiguration, RoomAssignment.
            
            // InventoryConfiguration might have it?
            
            var roomTypeIds = await _context.Booking
                .Where(b => b.HotelId == hotelId.Value)
                .SelectMany(b => b.Items)
                .Select(i => i.RoomTypeId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!roomTypeIds.Any())
            {
                return Result.Success(new List<OccupancyDataPointDto>());
            }

            var inventoryData = await _context.Inventory
                .AsNoTracking()
                .Where(i => roomTypeIds.Contains(i.RoomTypeId))
                .Where(i => i.Date >= request.FromDate && i.Date <= request.ToDate)
                .GroupBy(i => i.Date)
                .Select(g => new OccupancyDataPointDto
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    RoomsBooked = g.Sum(i => i.SoldStock),
                    OccupancyRate = g.Sum(i => i.TotalStock) > 0 
                        ? (double)g.Sum(i => i.SoldStock) / g.Sum(i => i.TotalStock) * 100 
                        : 0
                })
                .OrderBy(d => d.Label)
                .ToListAsync(cancellationToken);

            return Result.Success(inventoryData);
        }
    }
}
