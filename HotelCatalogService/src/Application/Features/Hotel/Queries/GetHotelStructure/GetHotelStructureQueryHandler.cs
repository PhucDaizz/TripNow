using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelStructure
{
    public class GetHotelStructureQueryHandler : IRequestHandler<GetHotelStructureQuery, Result<List<BlockResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetHotelStructureQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<BlockResponseDto>>> Handle(GetHotelStructureQuery request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithFullStructureAsync(request.HotelId, token);

            if (hotel == null)
                return Result.Failure<List<BlockResponseDto>>(new Error("Hotel.NotFound", "Hotel not found!"));

            var structure = hotel.Blocks.Select(b => new BlockResponseDto(
                Id: b.Id,
                BlockName: b.Name,
                Floors: b.Floors.Select(f => new FloorResponseDto(
                    Id: f.Id,
                    FloorName: f.FloorNumber, 
                    Rooms: f.Rooms.Select(r => new RoomResponseDto(
                        Id: r.Id,
                        RoomName: r.RoomName,
                        RoomTypeId: r.RoomTypeId
                    )).ToList()
                )).ToList()
            )).ToList();

            return Result.Success(structure);
        }
    }
}
