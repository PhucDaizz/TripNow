using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.MaintainRoom
{
    public class MaintainRoomCommand: IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid BlockId { get; set; }
        public Guid FloorId { get; set; }
        public Guid RoomId { get; set; }
        public Guid OwnerId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
    }
}
