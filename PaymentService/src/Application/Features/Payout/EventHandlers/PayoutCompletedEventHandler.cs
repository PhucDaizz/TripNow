using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Common;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Events.Payout;

namespace PaymentService.Application.Features.Payout.EventHandlers
{
    public class PayoutCompletedEventHandler : INotificationHandler<PayoutCompletedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventService _integrationEventService;

        public PayoutCompletedEventHandler(IUnitOfWork unitOfWork, IIntegrationEventService integrationEventService)
        {
            _unitOfWork = unitOfWork;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(PayoutCompletedEvent notification, CancellationToken cancellationToken)
        {
            var wallet = await _unitOfWork.OwnerWallets.GetByIdAsync(notification.OwnerWalletId, cancellationToken);
            if (wallet == null) return;

            var notificationEvent = new SystemNotificationCreateEvent
            {
                OwnerId = wallet.OwnerId,
                Title = "Rút tiền thành công",
                Message = $"Yêu cầu rút {notification.Amount:N0}đ đã được xử lý thành công. Tiền đang được chuyển đến tài khoản của bạn. Mã giao dịch: {notification.TransactionReference}",
                Type = NotificationType.Payment, 
                ReferenceId = notification.PayoutId.ToString(),
                IsHotelNotification = true
            };

            await _integrationEventService.PublishAsync<SystemNotificationCreateEvent>(
                notificationEvent,
                "payment.events",
                "topic",
                "new.notification.system",
                cancellationToken);
        }
    }
}
