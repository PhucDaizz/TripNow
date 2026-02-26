using BookingService.Application.Common.Interfaces;
using BookingService.Domain.Exceptions;
using Domain.Common.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.RoomAssignment.Commands.CheckOutRoom
{
    public class CheckOutRoomCommandHandler : IRequestHandler<CheckOutRoomCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHotelAuthorizationService _hotelAuthService;
        private readonly ILogger<CheckOutRoomCommandHandler> _logger;

        public CheckOutRoomCommandHandler(
            IUnitOfWork unitOfWork,
            IHotelAuthorizationService hotelAuthService,
            ILogger<CheckOutRoomCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _hotelAuthService = hotelAuthService;
            _logger = logger;
        }

        public async Task<Result> Handle(CheckOutRoomCommand request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.Booking.GetBookingWithDetailItemAssignmentAsync(request.BookingId, cancellationToken);

            if (booking == null)
            {
                return Result.Failure(new Error("Booking.NotFound", "No reservation found."));
            }

            bool hasAccess = await _hotelAuthService.HasHotelAccessAsync(booking.HotelId, cancellationToken);
            if (!hasAccess)
            {
                _logger.LogWarning("Unauthorized checkout attempt! User tried to checkout Room {RoomId} of Hotel {HotelId}", request.RoomId, booking.HotelId);
                return Result.Failure(new Error("Auth.Forbidden", "You do not have permission to perform check-out for this hotel."));
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
                booking.CheckOutRoom(request.RoomId);

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
