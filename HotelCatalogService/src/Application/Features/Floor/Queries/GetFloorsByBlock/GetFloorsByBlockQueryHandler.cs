using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Floor;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Floor.Queries.GetFloorsByBlock
{
    public class GetFloorsByBlockQueryHandler : IRequestHandler<GetFloorsByBlockQuery, Result<List<FloorDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationDbContext _context;

        public GetFloorsByBlockQueryHandler(IUnitOfWork unitOfWork, IApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }


        public async Task<Result<List<FloorDto>>> Handle(GetFloorsByBlockQuery request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithBlocksAndFloorsAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure<List<FloorDto>>(new Error("Hotel.NotFound", "Not found"));

            var block = hotel.Blocks.FirstOrDefault(b => b.Id == request.BlockId);
            if (block == null) return Result.Failure<List<FloorDto>>(new Error("Block.NotFound", "Not found"));

            var floorIds = block.Floors.Select(f => f.Id).ToList();

            var roomCounts = await _context.Rooms.AsNoTracking()
                .Where(r => floorIds.Contains(r.FloorId))
                .GroupBy(r => r.FloorId)
                .Select(g => new { FloorId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.FloorId, v => v.Count, token);

            var dtos = block.Floors
                .OrderBy(f => f.FloorNumber)
                .Select(f => new FloorDto
                {
                    Id = f.Id,
                    FloorNumber = f.FloorNumber,
                    RoomCount = roomCounts.GetValueOrDefault(f.Id, 0)
                }).ToList();

            return Result.Success(dtos);
        }
    }
}
