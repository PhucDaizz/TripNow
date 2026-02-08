using Application.Common.Interfaces;
using Application.Contracts;
using Application.DTOs.Payment;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.User.EventHandlers
{
    public class RefundRequestCompletedEventHandler : INotificationHandler<RefundRequestCompleted>
    {
        private readonly IEmailServices _emailServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RefundRequestCompletedEventHandler> _logger;

        public RefundRequestCompletedEventHandler(IEmailServices emailServices, IUnitOfWork unitOfWork, ILogger<RefundRequestCompletedEventHandler> logger)
        {
            _emailServices = emailServices;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(RefundRequestCompleted notification, CancellationToken cancellationToken)
        {
            var useRefund = await _unitOfWork.Auth.GetUserByIdAsync(notification.UserRefundId.ToString());

            if (useRefund != null){
                var htmlBody = _emailServices.CreateRefundSuccessEmailBody(useRefund.FullName, useRefund.Email, notification.AmountRefund, notification.RefundId.ToString());
                var emailSent = await _emailServices.SendEmailAsync(useRefund.Email, $"[TripUp] Xác nhận hoàn tiền thành công - Mã giao dịch {notification.RefundId.ToString()}", htmlBody, true);

                if (emailSent)
                {
                    _logger.LogInformation("Confirmation email sent successfully to: {UserEmail}", useRefund.Email);
                }
                else
                {
                    _logger.LogWarning("Failed to send confirmation email to: {UserEmail}", useRefund.Email);
                }
            }

        }
    }
}
