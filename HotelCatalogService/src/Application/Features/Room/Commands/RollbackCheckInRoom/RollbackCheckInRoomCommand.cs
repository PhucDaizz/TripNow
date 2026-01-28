using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.RollbackCheckInRoom
{
    public class RollbackCheckInRoomCommand: IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid RoomId { get; set; }
    }
}
