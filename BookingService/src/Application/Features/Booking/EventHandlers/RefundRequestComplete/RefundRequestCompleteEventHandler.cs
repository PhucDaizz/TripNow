using BookingService.Application.Common.Interfaces;
using BookingService.Domain.Events.Booking;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.Booking.EventHandlers.RefundRequestComplete
{
    public class RefundRequestCompleteEventHandler : INotificationHandler<RefundRequestCompleteEvent>
    {
        private readonly ILogger<RefundRequestCompleteEventHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public RefundRequestCompleteEventHandler(ILogger<RefundRequestCompleteEventHandler> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RefundRequestCompleteEvent notification, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.Booking.GetBookingByIdAsync(notification.BookingId, cancellationToken);

            if (booking is null)
            {
                _logger.LogError("Booking with id {BookingId} not found", notification.BookingId);
                return;
            }

            booking.CancelCompletely();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
