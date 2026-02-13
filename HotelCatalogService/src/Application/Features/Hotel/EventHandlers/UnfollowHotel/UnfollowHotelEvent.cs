using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.EventHandlers.UnfollowHotel
{
    public class UnfollowHotelEvent: INotification
    {
        public Guid HotelId { get; set; }
    }
}
