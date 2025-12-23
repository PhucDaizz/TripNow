using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.DeleteHotel
{
    public class DeleteHotelCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid OwnerId { get; set; } 
    }
}
