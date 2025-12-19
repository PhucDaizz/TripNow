using Application.Common.Interfaces;
using Application.Contracts;
using Application.DTOs.StaffProfile;
using Domain.Common;
using Domain.Common.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.StaffProfile.Commands.CreateStaffProfile
{
    public class CreateStaffProfileCommandHandler : IRequestHandler<CreateStaffProfileCommand, Result<StaffProfileDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly ILogger<CreateStaffProfileCommandHandler> _logger;

        public CreateStaffProfileCommandHandler(
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            ILogger<CreateStaffProfileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<Result<StaffProfileDto>> Handle(
            CreateStaffProfileCommand request,
            CancellationToken cancellationToken)
        {

            if (!AppRoles.IsValidPosition(request.Position))
                return Result.Failure<StaffProfileDto>(
                    new Error("INVALID.POSITION",
                        $"Invalid position. Valid positions: {string.Join(", ", AppRoles.AllRoles)}"));

            var creatorRoles = await _identityService
                .GetRolesAsync(request.CreatedByUserId);

            var isSysAdmin = creatorRoles.Contains(AppRoles.SysAdmin);
            var isHotelOwner = creatorRoles.Contains(AppRoles.HotelOwner);

            var targetRole = AppRoles.GetRoleForPosition(request.Position);


            if (isHotelOwner && !isSysAdmin && targetRole == AppRoles.SysAdmin)
            {
                return Result.Failure<StaffProfileDto>(
                    new Error("FORBIDDEN.ROLE_ASSIGN",
                        "HotelOwner is not allowed to assign SysAdmin role"));
            }

            // Check if user exists
            var user = await _unitOfWork.Auth.GetUserByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                return Result.Failure<StaffProfileDto>(
                    new Error("USER.NOT_FOUND", "User not found"));

            // Check if staff profile already exists for this user
            var existingProfile = await _unitOfWork.StaffProfile.GetByUserIdAsync(user.Id, cancellationToken);
            if (existingProfile != null)
                return Result.Failure<StaffProfileDto>(
                    new Error("STAFF.EXISTS", "Staff profile already exists for this user"));


            var roleResult = await _identityService.AssignRoleAsync(user.Id, targetRole, cancellationToken);
            if (roleResult.IsFailure)
                return Result.Failure<StaffProfileDto>(roleResult.Error);

            await _identityService.RemoveRoleAsync(user.Id, AppRoles.Customer, cancellationToken);

            // Create staff profile
            var staffProfile = new Domain.Entities.StaffProfile
            {
                UserId = user.Id,
                HotelId = request.HotelId,
                Position = targetRole,
                CreatedBy = request.CreatedByUserId
            };

            var createdProfile = await _unitOfWork.StaffProfile.CreateAsync(staffProfile, cancellationToken);

            _logger.LogInformation(
                "Created staff profile {ProfileId} by {CreatorId} for user {UserId} as {Role}",
                createdProfile.Id,
                request.CreatedByUserId,
                user.Id,
                targetRole);

            return Result<StaffProfileDto>.Success(MapToDto(createdProfile));
        }

        private StaffProfileDto MapToDto(Domain.Entities.StaffProfile profile)
        {
            return new StaffProfileDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                HotelId = profile.HotelId,
                Position = profile.Position
            };
        }

        
    }
}
