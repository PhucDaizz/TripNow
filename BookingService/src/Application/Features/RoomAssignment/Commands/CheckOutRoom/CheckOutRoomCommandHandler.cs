using BookingService.Application.Common.Interfaces;
using BookingService.Application.Contracts;
using BookingService.Domain.Exceptions;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.RoomAssignment.Commands.CheckOutRoom
{
    public class CheckOutRoomCommandHandler : IRequestHandler<CheckOutRoomCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckOutRoomCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CheckOutRoomCommand request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.Booking.GetBookingWithDetailItemAssignmentAsync(request.BookingId, cancellationToken);

            if (booking == null)
            {
                return Result.Failure(new Error("Booking.NotFound", "No reservation found."));
            }

            var assignment = booking.Items
                .SelectMany(i => i.Assignments)
                .FirstOrDefault(a => a.RoomId == request.RoomId);

            if (assignment == null)
            {
                return Result.Failure(new Error("Assignment.NotFound", "This room does not belong to this reservation."));
            }
            try
            {
                assignment.CheckOut();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException ex)
            {
                return Result.Failure(new Error("Domain.Error", ex.Message));
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("System.Error", ex.Message));
            }
        }
    }
}
