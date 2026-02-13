using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.IsHotelExisting
{
    public class IsHotelExistingQuery: IRequest<Result<bool>>
    {
        public Guid HotelId { get; set; }
    }
}
