using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.CreateRoomType
{
    public class CreateRoomTypeCommand : IRequest<Result<Guid>>
    {
        public Guid HotelId { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public int Capacity { get; set; }
        public decimal SizeM2 { get; set; }
    }
}
