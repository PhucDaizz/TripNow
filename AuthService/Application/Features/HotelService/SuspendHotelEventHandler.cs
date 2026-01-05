using Application.Common.Interfaces;
using Application.Contracts;
using Application.DTOs.Hotel;
using MediatR;

namespace Application.Features.HotelService
{
    public class SuspendHotelEventHandler : INotificationHandler<SuspendHotelEvent>
    {
        private readonly IEmailServices _emailServices;
        private readonly IUnitOfWork _unitOfWork;
        public SuspendHotelEventHandler(IEmailServices emailServices, IUnitOfWork unitOfWork)
        {
            _emailServices = emailServices;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(SuspendHotelEvent notification, CancellationToken cancellationToken)
        {
            var owner = await _unitOfWork.Auth.GetUserByIdAsync(notification.OwnerId.ToString());

            if (owner != null)
            {
                string htmlBody = _emailServices.CreateHotelSuspendedEmailBody(owner.FullName, notification.HotelName, notification.Reason);
                var emailSent = await _emailServices.SendEmailAsync(owner.Email, "Khách sạn của bạn đã bị tạm thời khoá", htmlBody, true);
            }
        }
    }
}
