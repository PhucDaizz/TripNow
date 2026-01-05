using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.SuspendHotel
{
    public class SuspendHotelCommand: IRequest<Result>
    {
        public Guid HotelId { get; init; }
        public Guid? AdminId { get; init; }
        public string Reason { get; init; }
    }
}
