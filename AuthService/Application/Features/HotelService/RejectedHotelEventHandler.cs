using Application.Common.Interfaces;
using Application.Contracts;
using Application.DTOs.Hotel;
using MediatR;

namespace Application.Features.HotelService
{
    public class RejectedHotelEventHandler : INotificationHandler<RejectedHotelEvent>
    {
        private readonly IEmailServices _emailServices;
        private readonly IUnitOfWork _unitOfWork;

        public RejectedHotelEventHandler(IEmailServices emailServices, IUnitOfWork unitOfWork)
        {
            _emailServices = emailServices;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RejectedHotelEvent notification, CancellationToken cancellationToken)
        {
            var owner = await _unitOfWork.Auth.GetUserByIdAsync(notification.OwnerId.ToString());

            if(owner != null)
            {
                string htmlBody = _emailServices.CreateHotelRejectedEmailBody(owner.FullName, notification.HotelName, notification.Reason);
                var emailSent = await _emailServices.SendEmailAsync(owner.Email!, "Khách sạn của bạn bị từ chối phê duyệt", htmlBody, true);
            }
        }
    }
}
