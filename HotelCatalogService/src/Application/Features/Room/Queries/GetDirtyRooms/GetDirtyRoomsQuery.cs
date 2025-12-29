using Domain.Common.Response;
using HotelCatalogService.Domain.Dto.Room;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Queries.GetDirtyRooms
{
    public class GetDirtyRoomsQuery : IRequest<Result<List<DirtyRoomDto>>>
    {
        public Guid HotelId { get; set; }
        public Guid? BlockId { get; set; } 
        public Guid? FloorId { get; set; }
    }
}
