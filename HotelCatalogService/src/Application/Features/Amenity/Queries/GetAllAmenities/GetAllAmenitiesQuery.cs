using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Amenity;
using MediatR;

namespace HotelCatalogService.Application.Features.Amenity.Queries.GetAllAmenities
{
    public record GetAllAmenitiesQuery : IRequest<Result<List<AmenityDto>>>;
}
