using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Amenity;
using HotelCatalogService.Domain.Repositories;
using MediatR;

namespace HotelCatalogService.Application.Features.Amenity.Queries.GetAllAmenities
{
    public class GetAllAmenitiesQueryHandler : IRequestHandler<GetAllAmenitiesQuery, Result<List<AmenityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllAmenitiesQueryHandler(IAmenityRepository repo, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<AmenityDto>>> Handle(GetAllAmenitiesQuery request, CancellationToken token)
        {
            var entities = await _unitOfWork.Amenity.GetAllAsync(token);
            var dtos = entities.Select(x => new AmenityDto(x.Id, x.Name, x.Icon)).ToList();
            return Result.Success(dtos);
        }
    }
}
