using BookingService.Application.DTOs.RoomAssignment;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.RoomAssignment.Queries.GetAssignedRooms
{
    public class GetAssignedRoomsQuery : IRequest<Result<List<AssignedRoomDto>>>
    {
        public Guid BookingId { get; set; }
    }
}
