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
        private readonly IHotelAuthorizationService _hotelAuthService;

        public CancelBookingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IHotelAuthorizationService hotelAuthService)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _hotelAuthService = hotelAuthService;
        }

        public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.Booking.GetBookingWithDetailItemAsync(request.BookingId, cancellationToken);

            if (booking is null)
            {
                return Result.Failure(
                    new Error("Booking.NotFound", "Booking not found."));
            }

            if (!await CanCancelBooking(booking, request.CancelledBy, cancellationToken))
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

        private async Task<bool> CanCancelBooking(Domain.Entities.Booking booking, CancelledBy cancelledBy, CancellationToken cancellationToken)
        {
            if (cancelledBy == CancelledBy.System)
                return true;

            if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.UserId))
                return false;

            if (_currentUser.Role == AppRoles.Customer)
            {
                return string.Equals(_currentUser.UserId, booking.UserId.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            return await _hotelAuthService.HasHotelAccessAsync(booking.HotelId, cancellationToken);
        }

    }
}
