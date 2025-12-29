using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Dto.Room;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Queries.GetDirtyRooms
{
    public class GetDirtyRoomsQueryHandler : IRequestHandler<GetDirtyRoomsQuery, Result<List<DirtyRoomDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDirtyRoomsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<DirtyRoomDto>>> Handle(GetDirtyRoomsQuery request, CancellationToken token)
        {
            var rooms = await _unitOfWork.Hotel.GetDirtyRoomsAsync(request.HotelId, request.BlockId, request.FloorId, token);
            return Result.Success(rooms);
        }
    }
}
