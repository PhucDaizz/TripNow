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
            var prices = await _context.RoomPrice
                .Where(p => EF.Property<Guid>(p, "RoomTypeId") == request.RoomTypeId
                         && p.Date >= request.FromDate.Date
                         && p.Date <= request.ToDate.Date)
                .OrderBy(p => p.Date)
                .Select(p => new RoomPriceDto
                {
                    Date = p.Date,
                    Price = p.Price
                })
                .ToListAsync(token);

            return Result.Success(prices);
        }
    }
}
