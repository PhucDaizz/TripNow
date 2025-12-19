using Application.Common.Interfaces;
using Application.Contracts;
using Domain.Common;
using Domain.Common.Response;
using MediatR;

namespace Application.Features.StaffProfile.Commands.DeleteStaffProfile
{
    public class DeleteStaffProfileCommandHandler : IRequestHandler<DeleteStaffProfileCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public DeleteStaffProfileCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<Result> Handle(
            DeleteStaffProfileCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Get staff profile
            var staffProfile = await _unitOfWork.StaffProfile.GetByUserIdAsync(request.UserId.ToString(), cancellationToken);
            if (staffProfile == null)
                return Result.Failure(new Error("STAFF.NOT_FOUND", "Staff profile not found"));

            // 2. Get role for position
            var targetRole = AppRoles.GetRoleForPosition(staffProfile.Position);

            var deleterRoles = await _identityService
                .GetRolesAsync(request.DeletedByUserId);

            var isSysAdmin = deleterRoles.Contains(AppRoles.SysAdmin);
            var isHotelOwner = deleterRoles.Contains(AppRoles.HotelOwner);

            if (isHotelOwner && !isSysAdmin && targetRole == AppRoles.SysAdmin)
            {
                return Result.Failure(
                    new Error("FORBIDDEN.DELETE",
                        "HotelOwner is not allowed to delete SysAdmin staff"));
            }

            if (isHotelOwner && !isSysAdmin)
            {
                var owner = await _unitOfWork.StaffProfile.GetByIdAsync(Guid.Parse(request.DeletedByUserId), cancellationToken);

                if (owner == null)
                    return Result.Failure(
                        new Error("USER.NOT_FOUND", "Owner not found"));

                if (owner.HotelId != staffProfile.HotelId)
                {
                    return Result.Failure(
                        new Error("FORBIDDEN.HOTEL_SCOPE",
                            "You are not allowed to delete staff from another hotel"));
                }
            }

            // 3. Remove staff role
            await _identityService.RemoveRoleAsync(staffProfile.UserId, targetRole, cancellationToken);

            // 4. Add customer role back
            await _identityService.AssignRoleAsync(staffProfile.UserId, AppRoles.Customer, cancellationToken);

            // 5. Delete staff profile
            var deleted = await _unitOfWork.StaffProfile.DeleteByUserIdAsync(request.UserId.ToString(), cancellationToken);

            if (!deleted)
                return Result.Failure(new Error("DELETE.FAILED", "Failed to delete staff profile"));

            return Result.Success("Staff profile deleted and user role reverted to Customer");
        }
    }
}
