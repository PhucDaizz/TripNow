using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Booking.Event;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.Booking.EventHandlers.PaymentSucceeded
{
    public class PaymentSucceededEventHandler : INotificationHandler<PaymentSucceededEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentSucceededEventHandler> _logger;

        public PaymentSucceededEventHandler(IUnitOfWork unitOfWork, ILogger<PaymentSucceededEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(PaymentSucceededEvent notification, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.Booking.GetBookingByIdAsync(notification.BookingId, cancellationToken);
            if (booking == null)
            {
                _logger.LogWarning("Booking with ID {BookingId} not found.", notification.BookingId);
                return;
            }

            booking.PaymentSusscess();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
