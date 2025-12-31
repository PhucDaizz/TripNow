using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomPrice.Commands.SetRoomPrice
{
    public class SetRoomPriceCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }
}
