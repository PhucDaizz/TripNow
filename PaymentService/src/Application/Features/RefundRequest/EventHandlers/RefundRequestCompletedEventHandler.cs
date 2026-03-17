using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.Common;
using PaymentService.Application.DTOs.RefundRequest;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Events.RefundRequest;

namespace PaymentService.Application.Features.RefundRequest.EventHandlers
{
    public class RefundRequestCompletedEventHandler : INotificationHandler<RefundRequestCompletedEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;

        public RefundRequestCompletedEventHandler(IIntegrationEventService integrationEventService)
        {
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(RefundRequestCompletedEvent notification, CancellationToken cancellationToken)
        {
            var emailEventTask = _integrationEventService.PublishAsync<RefundRequestCompleted>(
                new RefundRequestCompleted
                {
                    RefundId = notification.id,
                    BookingId = notification.bookingId,
                    UserRefundId = notification.useRefundId,
                    AmountRefund = notification.amountRefund
                },
                "payment.events",
                "topic",
                "refundrequest.completed",
                cancellationToken
            );

            var systemNotificationTask = _integrationEventService.PublishAsync<SystemNotificationCreateEvent>(
                new SystemNotificationCreateEvent
                {
                    OwnerId = notification.useRefundId, 
                    Title = "Hoàn tiền thành công",
                    Message = $"Số tiền {notification.amountRefund:N0}đ cho đơn đặt phòng của bạn đã được hoàn trả thành công.",
                    Type = NotificationType.Payment,
                    ReferenceId = notification.bookingId.ToString(), 
                    IsHotelNotification = false 
                },
                "payment.events", 
                "topic",
                "new.notification.system",
                cancellationToken
            );

            await Task.WhenAll(emailEventTask, systemNotificationTask);
        }
    }
}
