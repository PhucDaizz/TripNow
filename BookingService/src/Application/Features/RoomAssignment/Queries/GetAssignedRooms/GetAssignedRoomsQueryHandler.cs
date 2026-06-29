using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.RoomAssignment;
using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Application.Features.RoomAssignment.Queries.GetAssignedRooms
{
    public class GetAssignedRoomsQueryHandler : IRequestHandler<GetAssignedRoomsQuery, Result<List<AssignedRoomDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAssignedRoomsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<AssignedRoomDto>>> Handle(GetAssignedRoomsQuery request, CancellationToken cancellationToken)
        {
            var bookingExists = await _context.Bookings.AnyAsync(b => b.Id == request.BookingId, cancellationToken);
            if (!bookingExists)
            {
                return Result.Failure<List<AssignedRoomDto>>(new Error("Booking.NotFound", "Can not found Booking."));
            }

            var assignedRooms = await _context.BookingItems
                .AsNoTracking()
                .Where(bi => bi.BookingId == request.BookingId)
                .SelectMany(
                    bi => bi.Assignments,
                    (bi, assignment) => new AssignedRoomDto
                    {
                        BookingItemId = bi.Id,
                        RoomTypeId = bi.RoomTypeId,
                        RoomId = assignment.RoomId,
                        RoomName = assignment.RoomName,
                        IsCheckedIn = assignment.IsCheckedIn,
                        CheckInTime = assignment.CheckInTime,
                        CheckOutTime = assignment.CheckOutTime
                    }
                )
                .ToListAsync(cancellationToken);

            return Result.Success(assignedRooms);
        }
    }
}
