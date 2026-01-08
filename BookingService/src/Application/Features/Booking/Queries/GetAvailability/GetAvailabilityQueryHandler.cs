using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Inventory;
using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Application.Features.Booking.Queries.GetAvailability
{
    public class GetAvailabilityQueryHandler : IRequestHandler<GetAvailabilityQuery, Result<List<DailyAvailabilityDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAvailabilityQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<DailyAvailabilityDto>>> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
        {
            DateOnly fromDate, toDate;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (request.CheckInDate.HasValue && request.CheckOutDate.HasValue)
            {
                fromDate = request.CheckInDate.Value;
                // Trừ 1 ngày vì ngày Check-out không cần giữ phòng qua đêm đó
                toDate = request.CheckOutDate.Value.AddDays(-1);

                if (fromDate > toDate)
                    return Result.Failure<List<DailyAvailabilityDto>>(new Error("Inventory.InvalidDate", "Ngày Check-in phải trước ngày Check-out"));
            }
            else
            {
                // Logic mặc định: Tìm theo tháng
                int year = request.Year ?? today.Year;
                int month = request.Month ?? today.Month;

                // Validate tháng năm
                if (month < 1 || month > 12)
                    return Result.Failure<List<DailyAvailabilityDto>>(new Error("Inventory.InvalidMonth", "Tháng không hợp lệ"));

                try
                {
                    fromDate = new DateOnly(year, month, 1);
                    toDate = fromDate.AddMonths(1).AddDays(-1); // Ngày cuối tháng
                }
                catch
                {
                    return Result.Failure<List<DailyAvailabilityDto>>(new Error("Inventory.InvalidDate", "Thời gian không hợp lệ"));
                }
            }

            // 2. Logic chặn xem quá khứ
            // Nếu khách chọn xem tháng quá khứ (VD: Nay tháng 10, khách chọn tháng 9)
            if (toDate < today)
            {
                return Result.Success(new List<DailyAvailabilityDto>());
            }

            if (fromDate < today)
            {
                fromDate = today;
            }

            // 3. Query Database
            var inventoryData = await _context.Inventory.AsQueryable()
                .AsNoTracking()
                .Where(x => x.RoomTypeId == request.RoomTypeId
                            && x.Date >= fromDate
                            && x.Date <= toDate)
                .Select(x => new DailyAvailabilityDto
                {
                    Date = x.Date,
                    TotalStock = x.TotalStock,
                    SoldStock = x.SoldStock,
                    BlockedStock = x.BlockedStock
                })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            return Result.Success(inventoryData);
        }
    }
}
