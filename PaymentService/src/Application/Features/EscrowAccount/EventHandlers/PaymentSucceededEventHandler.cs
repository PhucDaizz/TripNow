using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Domain.Events.PaymentTransaction;

namespace PaymentService.Application.Features.EscrowAccount.EventHandlers
{
    public class PaymentSucceededEventHandler : INotificationHandler<PaymentSucceededEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentSucceededEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(PaymentSucceededEvent notification, CancellationToken cancellationToken)
        {
            var newEscrowAccountEntry = new Domain.Entities.EscrowAccount(
                notification.BookingId,
                notification.Amount,
                notification.ProviderFee
            );

            await _unitOfWork.EscrowAccounts.AddAsync(newEscrowAccountEntry);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
