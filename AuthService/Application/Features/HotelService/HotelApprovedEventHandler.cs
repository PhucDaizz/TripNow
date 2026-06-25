using Application.Common.Interfaces;
using Application.Contracts;
using Application.DTOs.Hotel;
using MediatR;

namespace Application.Features.HotelService
{
    public class HotelApprovedEventHandler : INotificationHandler<HotelApproved>
    {
        private readonly IEmailServices _emailServices;
        private readonly IUnitOfWork _unitOfWork;

        public HotelApprovedEventHandler(IEmailServices emailServices, IUnitOfWork unitOfWork)
        {
            _emailServices = emailServices;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(HotelApproved notification, CancellationToken cancellationToken)
        {
            var owner = await _unitOfWork.Auth.GetUserByIdAsync(notification.OwnerId);

            if (owner is not null)
            {
                string htmlBody = _emailServices.CreateHotelApprovedEmailBody(owner.FullName, notification.HotelName);
                var emailSent = await _emailServices.SendEmailAsync(owner.Email!, "🎉 Chúc mừng! Khách sạn của bạn đã được phê duyệt", htmlBody, true);
            }
        }
    }
}
