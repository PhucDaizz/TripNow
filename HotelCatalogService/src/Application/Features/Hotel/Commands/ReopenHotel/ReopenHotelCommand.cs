using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.ReopenHotel
{
    public class ReopenHotelCommand: IRequest<Result>
    {
        public Guid HotelId { get; init; }
        public Guid OwerId { get; init; }
    }
}
