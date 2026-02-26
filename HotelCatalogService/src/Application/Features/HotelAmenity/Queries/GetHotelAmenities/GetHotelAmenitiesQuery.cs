using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.HotelAmenity;
using MediatR;

namespace HotelCatalogService.Application.Features.HotelAmenity.Queries.GetHotelAmenities
{
    public class GetHotelAmenitiesQuery : IRequest<Result<List<HotelAmenityDto>>>
    {
        public Guid HotelId { get; set; }
    }
}
