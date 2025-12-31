using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.RoomPrice;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.RoomPrice.Queries.GetRoomTypeCalendar
{
    public class GetRoomTypeCalendarQueryHandler : IRequestHandler<GetRoomTypeCalendarQuery, Result<List<CalendarDayDto>>>
    {
        private readonly IApplicationDbContext _context; 

        public GetRoomTypeCalendarQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<CalendarDayDto>>> Handle(GetRoomTypeCalendarQuery request, CancellationToken token)
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(request.Year, request.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(request.Year, request.Month);
            var lastDayOfMonth = new DateTime(request.Year, request.Month, daysInMonth);

            if (lastDayOfMonth < today)
            {
                return Result.Failure<List<CalendarDayDto>>(new Error("Date.Past", "It is not possible to view past price data."));
            }

            var startDateToScan = firstDayOfMonth < today ? today : firstDayOfMonth;

            var roomTypeInfo = await _context.RoomType
                .Where(rt => rt.Id == request.RoomTypeId)
                .Select(rt => new { rt.BasePrice })
                .FirstOrDefaultAsync(token);

            if (roomTypeInfo == null)
                return Result.Failure<List<CalendarDayDto>>(new Error("RoomType.NotFound", "No room type found"));

            var specialPrices = await _context.RoomPrice
                .Where(p => EF.Property<Guid>(p, "RoomTypeId") == request.RoomTypeId
                         && p.Date >= startDateToScan
                         && p.Date <= lastDayOfMonth)
                .ToDictionaryAsync(p => p.Date.Date, p => p.Price, token);

            var calendar = new List<CalendarDayDto>();

            for (DateTime date = startDateToScan; date <= lastDayOfMonth; date = date.AddDays(1))
            {
                decimal finalPrice = roomTypeInfo.BasePrice; 
                bool isSpecial = false;

                if (specialPrices.TryGetValue(date, out decimal specialPrice))
                {
                    finalPrice = specialPrice;
                    isSpecial = true;
                }

                calendar.Add(new CalendarDayDto
                {
                    Date = date,
                    Price = finalPrice,
                    IsSpecialPrice = isSpecial
                });
            }

            return Result.Success(calendar);
        }
    }
}
