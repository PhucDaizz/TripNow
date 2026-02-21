using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Common;
using SocialService.Domain.Entities;
using SocialService.Domain.ValueObject;

namespace SocialService.Application.Features.Locations.Commands.CreateLocation
{
    public class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateLocationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);
            var role = _currentUserService.Role;
            var isAdmin = role == AppRoles.SysAdmin? true: false;

            Result<Coordinates> coordsResult;
            try
            {
                var coords = new Coordinates(request.Latitude, request.Longitude);

                var location = new Location(
                    request.Name,
                    coords,
                    request.Address,
                    request.Type,
                    userId
                );

                if ( isAdmin )
                {
                    location.Verify();
                }

                await _unitOfWork.locationRepository.AddAsync(location);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(location.Id);
            }
            catch (Exception ex)
            {
                return Result.Failure<Guid>(new Error("ERROR", ex.Message.ToString()));
            }
        }
    }
}
