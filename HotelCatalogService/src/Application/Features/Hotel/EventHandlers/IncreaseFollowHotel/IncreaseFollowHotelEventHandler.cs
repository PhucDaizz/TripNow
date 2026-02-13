using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.EventHandlers.IncreaseFollowHotel
{
    internal class IncreaseFollowHotelEventHandler : INotificationHandler<IncreaseFollowHotelEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public IncreaseFollowHotelEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(IncreaseFollowHotelEvent notification, CancellationToken cancellationToken)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdAsync(notification.HotelId);

            if (hotel != null)
            {
                hotel.FollowHotel();
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
