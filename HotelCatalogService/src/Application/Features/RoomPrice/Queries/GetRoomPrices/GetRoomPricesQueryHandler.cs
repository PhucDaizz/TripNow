using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.RoomPrice;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.RoomPrice.Queries.GetRoomPrices
{
    public class GetRoomPricesQueryHandler : IRequestHandler<GetRoomPricesQuery, Result<List<RoomPriceDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetRoomPricesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<RoomPriceDto>>> Handle(GetRoomPricesQuery request, CancellationToken token)
        {
            var roomTypeData = await _context.RoomType
                .Where(rt => rt.Id == request.RoomTypeId)
                .Select(rt => new { rt.BasePrice })
                .FirstOrDefaultAsync(token);

            if (roomTypeData == null)
            {
                return Result.Failure<List<RoomPriceDto>>(new Error("RoomType.NotFound", "This room type does not exist.."));
            }

            var specialPrices = await _context.RoomPrice
                .AsNoTracking()
                .Where(p => p.RoomTypeId == request.RoomTypeId
                         && p.Date >= request.FromDate.Date
                         && p.Date <= request.ToDate.Date)
                .ToDictionaryAsync(k => k.Date.Date, v => v.Price, token);

            var result = new List<RoomPriceDto>();

            for (var date = request.FromDate.Date; date <= request.ToDate.Date; date = date.AddDays(1))
            {
                decimal finalPrice = specialPrices.TryGetValue(date, out var specialPrice)
                    ? specialPrice
                    : roomTypeData.BasePrice;

                result.Add(new RoomPriceDto
                {
                    RoomTypeId = request.RoomTypeId,
                    Date = date,
                    Price = finalPrice
                });
            }

            return Result.Success(result);
        }
    }
}
