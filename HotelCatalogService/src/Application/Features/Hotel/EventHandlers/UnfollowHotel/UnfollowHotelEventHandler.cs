using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.EventHandlers.UnfollowHotel
{
    public class UnfollowHotelEventHandler : INotificationHandler<UnfollowHotelEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnfollowHotelEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UnfollowHotelEvent notification, CancellationToken cancellationToken)
        {
            var hotel = await _unitOfWork.Hotel.GetByIdAsync(notification.HotelId, cancellationToken);
            if (hotel != null)
            {
                hotel.UnFollow();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
