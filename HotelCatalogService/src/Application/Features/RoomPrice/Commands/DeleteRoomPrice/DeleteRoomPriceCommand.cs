using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomPrice.Commands.DeleteRoomPrice
{
    public class DeleteRoomPriceCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime Date { get; set; } 
    }
}
