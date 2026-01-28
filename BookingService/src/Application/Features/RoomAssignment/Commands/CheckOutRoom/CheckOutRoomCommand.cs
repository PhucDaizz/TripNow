using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.RoomAssignment.Commands.CheckOutRoom
{
    public class CheckOutRoomCommand: IRequest<Result>
    {
        public Guid BookingId { get; set; }
        public Guid RoomId { get; set; }
    }
}
