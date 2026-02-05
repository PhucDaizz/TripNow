using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Payment.Event;

namespace PaymentService.Application.Features.Payment.EventHandlers
{
    public class BookingCancelledWithOutPayEventHandler : INotificationHandler<BookingCancelledWithOutPay>
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingCancelledWithOutPayEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(BookingCancelledWithOutPay notification, CancellationToken cancellationToken)
        {
            var paymentTransaction = await _unitOfWork.PaymentTransactions.GetByBookingIdAsync(notification.BookingId, cancellationToken);

            if (paymentTransaction != null)
            {
                paymentTransaction.Cancel(notification.Reason);
            }
        }
    }
}
