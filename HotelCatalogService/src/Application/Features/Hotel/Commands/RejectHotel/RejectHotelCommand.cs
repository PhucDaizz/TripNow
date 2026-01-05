using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.RejectHotel
{
    public class RejectHotelCommand: IRequest<Result>
    {
        public Guid HotelId { get; init; }
        public Guid? AdminId { get; init; }
        public string Reason { get; init; }
    }
}
