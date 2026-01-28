using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.RoomAssignment.Commands.CheckInRoom
{
    public class CheckInRoomCommand: IRequest<Result>
    {
        public Guid BookingId { get; set; }
        public Guid HotelId { get; set; }
        public Guid RoomId { get; set; }
    }
}
