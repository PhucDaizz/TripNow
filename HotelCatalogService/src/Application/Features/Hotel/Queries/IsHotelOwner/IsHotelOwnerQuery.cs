using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.IsHotelOwner
{
    public class IsHotelOwnerQuery: IRequest<bool>
    {
        public Guid HotelId { get; set; }
        public Guid UserId { get; set; }
    }
}
