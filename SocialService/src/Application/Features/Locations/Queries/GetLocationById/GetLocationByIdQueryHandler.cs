using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.Locations;

namespace SocialService.Application.Features.Locations.Queries.GetLocationById
{
    public class GetLocationByIdQueryHandler : IRequestHandler<GetLocationByIdQuery, Result<LocationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLocationByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<LocationDto>> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
        {
            var location = await _unitOfWork.locationRepository.GetByIdAsync(request.Id, cancellationToken);
            if (location == null)
            {
                return Result.Failure<LocationDto>(new Error("NOT.FOUND","Location not found"));
            }
            var locationDto = new LocationDto
            {
                Id = request.Id,
                Name = location.Name,
                Address = location.Address,
                Latitude = location.Coordinates.Latitude,
                Longitude = location.Coordinates.Longitude,
                Type = location.Type.ToString(),
                AvgRating = location.AvgRating,
                IsVerify = location.IsVerify
            };

            return Result.Success(locationDto);
        }
    }
}
