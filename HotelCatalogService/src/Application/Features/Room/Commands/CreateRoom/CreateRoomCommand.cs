using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.CreateRoom
{
    public class CreateRoomCommand : IRequest<Result<Guid>>
    {
        public Guid HotelId { get; set; }
        public Guid BlockId { get; set; }
        public Guid FloorId { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; }
        public Guid RoomTypeId { get; set; }
    }
}
