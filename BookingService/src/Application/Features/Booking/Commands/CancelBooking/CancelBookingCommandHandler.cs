using BookingService.Application.Common.Interfaces;
using BookingService.Domain.Common;
using BookingService.Domain.Enum;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.Booking.Commands.CancelBooking
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CancelBookingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.Booking.GetBookingWithDetailItemAsync(request.BookingId, cancellationToken);

            if (booking is null)
            {
                return Result.Failure(
                    new Error("Booking.NotFound", "Booking not found."));
            }

            if (!CanCancelBooking(booking, request.CancelledBy))
                return Result.Failure(
                    new Error("Booking.Forbidden", "You are not allowed to cancel this booking."));


            decimal refundAmount = 0;
            RefundPolicy determinedPolicy = RefundPolicy.NonRefundable;
            if (booking.PaymentStatus == PaymentStatus.Paid)
            {
                refundAmount = booking.CalculateTotalRefundAmount(DateTime.UtcNow);
            }

            booking.Cancel(
               request.CancelledBy,
               request.Reason,
               refundAmount
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }

        private bool CanCancelBooking(Domain.Entities.Booking booking, CancelledBy cancelledBy)
        {
            if (cancelledBy == CancelledBy.System)
                return true;

            if (!_currentUser.IsAuthenticated)
                return false;

            return _currentUser.Role switch
            {
                AppRoles.SysAdmin => true,

                AppRoles.HotelOwner =>
                    _currentUser.HotelId == booking.HotelId,

                AppRoles.Receptionist =>
                    _currentUser.HotelId == booking.HotelId,

                AppRoles.Customer => string.Equals(_currentUser.UserId, booking.UserId.ToString(), StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }

    }
}
