using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Common;

namespace SocialService.Application.Features.Locations.Commands.VerifyLocation
{
    public class VerifyLocationCommandHandler : IRequestHandler<VerifyLocationCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public VerifyLocationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(VerifyLocationCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.Role != AppRoles.SysAdmin)
            {
                return Result.Failure<bool>(new Error("AUTH.FORBIDDEN", "Only Admin can verify locations."));
            }

            var location = await _unitOfWork.locationRepository.GetByIdAsync(request.Id);

            if (location == null)
            {
                return Result.Failure<bool>(new Error("LOCATION.NOTFOUND", "Location not found."));
            }

            if (location.IsVerify)
            {
                return Result.Failure<bool>(new Error("LOCATION.ALREADY_VERIFIED", "This location is already verified."));
            }

            location.Verify();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
    }
}
