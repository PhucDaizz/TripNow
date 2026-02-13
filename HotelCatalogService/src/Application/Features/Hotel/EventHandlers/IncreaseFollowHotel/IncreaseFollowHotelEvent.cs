using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.EventHandlers.IncreaseFollowHotel
{
    public class IncreaseFollowHotelEvent: INotification
    {
        public Guid HotelId { get; set; }
    }
}
