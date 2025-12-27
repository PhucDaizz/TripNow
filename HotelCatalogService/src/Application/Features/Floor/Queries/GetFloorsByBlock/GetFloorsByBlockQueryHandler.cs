using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Floor;
using MediatR;

namespace HotelCatalogService.Application.Features.Floor.Queries.GetFloorsByBlock
{
    public class GetFloorsByBlockQueryHandler : IRequestHandler<GetFloorsByBlockQuery, Result<List<FloorDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFloorsByBlockQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<List<FloorDto>>> Handle(GetFloorsByBlockQuery request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithBlocksAndFloorsAsync(request.HotelId, token);

            if (hotel == null) return Result.Failure<List<FloorDto>>(new Error("Hotel.NotFound", "Not found"));

            var block = hotel.Blocks.FirstOrDefault(b => b.Id == request.BlockId);
            if (block == null) return Result.Failure<List<FloorDto>>(new Error("Block.NotFound", "Not found"));

            var dtos = block.Floors
                .OrderBy(f => f.FloorNumber)
                .Select(f => new FloorDto
                {
                    Id = f.Id,
                    FloorNumber = f.FloorNumber,
                    RoomCount = f.Rooms?.Count ?? 0
                }).ToList();

            return Result.Success(dtos);
        }
    }
}
