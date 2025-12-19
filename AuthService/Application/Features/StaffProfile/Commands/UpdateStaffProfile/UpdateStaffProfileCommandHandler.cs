using Application.Common.Interfaces;
using Application.Contracts;
using Application.DTOs.StaffProfile;
using Domain.Common;
using Domain.Common.Response;
using MediatR;

namespace Application.Features.StaffProfile.Commands.UpdateStaffProfile
{
    public class UpdateStaffProfileCommandHandler
    : IRequestHandler<UpdateStaffProfileCommand, Result<StaffProfileDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public UpdateStaffProfileCommandHandler(
            IUnitOfWork unitOfWork,
            IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<Result<StaffProfileDto>> Handle(
            UpdateStaffProfileCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Validate position
            if (!AppRoles.IsValidPosition(request.NewPosition))
                return Result.Failure<StaffProfileDto>(
                    new Error("INVALID.POSITION", "Invalid position"));

            // 2. Get staff profile
            var staffProfile = await _unitOfWork.StaffProfile
                .GetByUserIdAsync(request.StaffProfileId.ToString(), cancellationToken);

            if (staffProfile == null)
                return Result.Failure<StaffProfileDto>(
                    new Error("STAFF.NOT_FOUND", "Staff profile not found"));

            // 3. Roles của người cập nhật
            var updaterRoles = await _identityService
                .GetRolesAsync(request.UpdatedByUserId);

            var isSysAdmin = updaterRoles.Contains(AppRoles.SysAdmin);
            var isHotelOwner = updaterRoles.Contains(AppRoles.HotelOwner);

            var targetNewRole = AppRoles.GetRoleForPosition(request.NewPosition);
            var currentRole = AppRoles.GetRoleForPosition(staffProfile.Position);

            // RULE 1: HotelOwner không được update SysAdmin
            if (isHotelOwner && !isSysAdmin && currentRole == AppRoles.SysAdmin)
            {
                return Result.Failure<StaffProfileDto>(
                    new Error("FORBIDDEN.UPDATE_ROLE",
                        "HotelOwner cannot update SysAdmin staff"));
            }

            // RULE 2: HotelOwner chỉ update staff cùng hotel
            if (isHotelOwner && !isSysAdmin)
            {
                var owner = await _unitOfWork.StaffProfile
                    .GetByUserIdAsync(request.UpdatedByUserId, cancellationToken);

                if (owner == null || owner.HotelId != staffProfile.HotelId)
                {
                    return Result.Failure<StaffProfileDto>(
                        new Error("FORBIDDEN.HOTEL_SCOPE",
                            "You cannot update staff from another hotel"));
                }
            }

            // 4. Update role nếu có thay đổi
            if (currentRole != targetNewRole)
            {
                await _identityService.RemoveRoleAsync(
                    staffProfile.UserId,
                    currentRole,
                    cancellationToken);

                await _identityService.AssignRoleAsync(
                    staffProfile.UserId,
                    targetNewRole,
                    cancellationToken);
            }

            // 5. Update staff profile
            staffProfile.Position = targetNewRole;
            staffProfile.HotelId = request.HotelId;
            staffProfile.UpdatedBy = request.UpdatedByUserId;

            await _unitOfWork.StaffProfile.UpdateAsync(
                staffProfile,
                cancellationToken);

            return Result<StaffProfileDto>.Success(
                new StaffProfileDto
                {
                    Id = staffProfile.Id,
                    UserId = staffProfile.UserId,
                    HotelId = staffProfile.HotelId,
                    Position = staffProfile.Position
                });
        }
    }

}
