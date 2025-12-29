using Application.Common.Interfaces;
using Application.DTOs.StaffProfile;
using Domain.Common.Response;
using MediatR;

namespace Application.Features.StaffProfile.Queries.GetStaffProfile
{
    public class GetStaffProfileQueryHandler : IRequestHandler<GetStaffProfileQuery, Result<StaffProfileDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStaffProfileQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<StaffProfileDto>> Handle(GetStaffProfileQuery request, CancellationToken cancellationToken)
        {
            var staffProfile = await _unitOfWork.StaffProfile.GetByUserIdAsync(request.UserId);
            if (staffProfile == null)
            {

                return Result.Failure<StaffProfileDto>(new Error("NOT.FOUND","Staff profile not found."));
            }
            var staffProfileDto = new StaffProfileDto
            {
                Id = staffProfile.Id,
                UserId = staffProfile.UserId,
                HotelId = staffProfile.HotelId,
                Position = staffProfile.Position
            };

            return Result.Success(staffProfileDto);
        }
    }
}
