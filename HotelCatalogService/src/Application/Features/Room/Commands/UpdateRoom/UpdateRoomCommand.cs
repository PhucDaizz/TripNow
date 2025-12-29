using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.UpdateRoom
{
    public class UpdateRoomCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid BlockId { get; set; }
        public Guid FloorId { get; set; }
        public Guid RoomId { get; set; }
        public Guid OwnerId { get; set; }

        public string Name { get; set; }
        public Guid RoomTypeId { get; set; }
    }
}
