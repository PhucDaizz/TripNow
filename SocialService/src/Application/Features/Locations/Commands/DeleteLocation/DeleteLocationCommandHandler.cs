using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Common;

namespace SocialService.Application.Features.Locations.Commands.DeleteLocation
{
    public class DeleteLocationCommandHandler : IRequestHandler<DeleteLocationCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteLocationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId);
            var role = _currentUserService.Role;

            var location = await _unitOfWork.locationRepository.GetByIdAsync(request.Id);
            if (location == null)
            {
                return Result.Failure<bool>(new Error("LOCATION.NOTFOUND","Location not existing"));
            }

            if (location.CreatedByUserId != currentUserId || role != AppRoles.SysAdmin)
                return Result.Failure<bool>(new Error("NOT.PERMIT","You are not permit"));

            await _unitOfWork.locationRepository.DeleteAsync(location, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
