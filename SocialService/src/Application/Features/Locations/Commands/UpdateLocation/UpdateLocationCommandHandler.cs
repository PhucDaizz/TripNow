using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Common;
using SocialService.Domain.ValueObject;

namespace SocialService.Application.Features.Locations.Commands.UpdateLocation
{
    public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateLocationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
        {
            var location = await _unitOfWork.locationRepository.GetByIdAsync(request.Id, cancellationToken);
            if (location == null) return Result.Failure<bool>(new Error("NOTFOUND.LOCATION", "Location not found."));

            var userId = Guid.Parse(_currentUserService.UserId);
            var role = _currentUserService.Role;

            if(location.CreatedByUserId != userId || role != AppRoles.SysAdmin)
            {
                if (location == null) return Result.Failure<bool>(new Error("NOT.PERMIT", "You are not permit"));
            }

            var newCoords = new Coordinates(request.Latitude, request.Longitude);

            location.UpdateDetails(request.Name, request.Address, newCoords, request.Type);

            await _unitOfWork.locationRepository.UpdateAsync(location, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
