using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.CancellationPolicy;
using HotelCatalogService.Application.DTOs.RoomPrice;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.RoomPrice.Queries.GetHotelBatchRoomPrices
{
    public class GetHotelBatchRoomPricesQueryHandler : IRequestHandler<GetHotelBatchRoomPricesQuery, Result<List<RoomTypeCalendarDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetHotelBatchRoomPricesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<RoomTypeCalendarDto>>> Handle(GetHotelBatchRoomPricesQuery request, CancellationToken token)
        {
            var roomTypes = await _context.RoomTypes
                .AsNoTracking()
                .Where(rt => rt.HotelId == request.HotelId)
                .Select(rt => new
                {
                    rt.Id,
                    rt.Name,
                    rt.BasePrice,
                    Policy = rt.CancellationPolicy == null ? null : new
                    {
                        rt.CancellationPolicy.Id,
                        rt.CancellationPolicy.Name,
                        rt.CancellationPolicy.Type,
                        Rules = rt.CancellationPolicy.Rules.Select(r => new
                        {
                            r.HoursBeforeCheckIn,
                            r.RefundPercentage
                        }).ToList()
                    }
                })
                .ToListAsync(token);

            if (!roomTypes.Any())
            {
                return Result.Failure<List<RoomTypeCalendarDto>>(new Error("Hotel.NoRooms", "Khách sạn này chưa có loại phòng nào."));
            }

            var roomTypeIds = roomTypes.Select(x => x.Id).ToList();

            var specialPricesRaw = await _context.RoomPrices
                .AsNoTracking()
                .Where(p => roomTypeIds.Contains(p.RoomTypeId) 
                         && p.Date >= request.FromDate.Date
                         && p.Date <= request.ToDate.Date)
                .ToListAsync(token);

            var specialPriceLookup = specialPricesRaw
                .GroupBy(x => x.RoomTypeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(p => p.Date.Date, p => p.Price)
                );

            var result = new List<RoomTypeCalendarDto>();

            foreach (var rt in roomTypes)
            {
                var calendarDto = new RoomTypeCalendarDto
                {
                    RoomTypeId = rt.Id,
                    RoomTypeName = rt.Name,
                    BasePrice = rt.BasePrice,
                    Calendar = new List<DailyPriceDto>(),
                    CancellationPolicy = rt.Policy == null ? null : new CancellationPolicyDto
                    {
                        Id = rt.Policy.Id,
                        Name = rt.Policy.Name,
                        Type = rt.Policy.Type.ToString(), 
                        Rules = rt.Policy.Rules.Select(r => new CancellationRuleDto
                        {
                            HoursBeforeCheckIn = r.HoursBeforeCheckIn,
                            RefundPercentage = r.RefundPercentage
                        })
                        .OrderByDescending(r => r.HoursBeforeCheckIn) 
                        .ToList()
                    }
                };

                var hasSpecialConfig = specialPriceLookup.TryGetValue(rt.Id, out var roomSpecialPrices);

                for (var date = request.FromDate.Date; date <= request.ToDate.Date; date = date.AddDays(1))
                {
                    decimal finalPrice = rt.BasePrice;
                    bool isSpecial = false;

                    if (hasSpecialConfig && roomSpecialPrices!.TryGetValue(date, out var spPrice))
                    {
                        finalPrice = spPrice;
                        isSpecial = true;
                    }

                    calendarDto.Calendar.Add(new DailyPriceDto
                    {
                        Date = date,
                        Price = finalPrice,
                        IsSpecialPrice = isSpecial
                    });
                }

                result.Add(calendarDto);
            }

            return Result.Success(result);
        }
    }
}
