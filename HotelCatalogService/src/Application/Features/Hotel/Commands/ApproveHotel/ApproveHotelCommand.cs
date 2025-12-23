using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.ApproveHotel
{
    public class ApproveHotelCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
    }
}
