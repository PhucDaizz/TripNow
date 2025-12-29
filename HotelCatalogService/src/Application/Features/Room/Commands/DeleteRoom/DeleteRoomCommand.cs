using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.DeleteRoom
{
    public class DeleteRoomCommand: IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid BlockId { get; set; }
        public Guid FloorId { get; set; }
        public Guid RoomId { get; set; }
        public Guid OwnerId { get; set; }
    }
}
