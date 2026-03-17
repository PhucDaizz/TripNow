using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Common;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Events.Payout;

namespace PaymentService.Application.Features.Payout.EventHandlers
{
    public class PayoutRejectedEventHandler : INotificationHandler<PayoutRejectedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventService _integrationEventService;

        public PayoutRejectedEventHandler(IUnitOfWork unitOfWork, IIntegrationEventService integrationEventService)
        {
            _unitOfWork = unitOfWork;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(PayoutRejectedEvent notification, CancellationToken cancellationToken)
        {
            var wallet = await _unitOfWork.OwnerWallets.GetByIdAsync(notification.OwnerWalletId, cancellationToken);
            if (wallet == null) return;

            var notificationEvent = new SystemNotificationCreateEvent
            {
                OwnerId = wallet.OwnerId,
                Title = "Yêu cầu rút tiền bị từ chối",
                Message = $"Yêu cầu rút {notification.Amount:N0}đ của bạn đã bị từ chối. Lý do: {notification.Reason}. Số tiền đã được hoàn lại vào ví.",
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
