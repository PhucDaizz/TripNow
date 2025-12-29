using Domain.Common.Response;
using HotelCatalogService.Domain.Enum;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.UpdateRoomStatus
{
    public class UpdateRoomStatusCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid BlockId { get; set; }
        public Guid FloorId { get; set; }
        public Guid RoomId { get; set; }
        public Guid OwnerId { get; set; }
        public RoomStatus NewStatus { get; set; }
    }
}
