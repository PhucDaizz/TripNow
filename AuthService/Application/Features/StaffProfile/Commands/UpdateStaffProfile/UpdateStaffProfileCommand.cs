using Application.DTOs.StaffProfile;
using Domain.Common.Response;
using MediatR;

namespace Application.Features.StaffProfile.Commands.UpdateStaffProfile
{
    public class UpdateStaffProfileCommand : IRequest<Result<StaffProfileDto>>
    {
        public Guid StaffProfileId { get; set; }
        public string NewPosition { get; set; } = default!;
        public Guid HotelId { get; set; }

        public string UpdatedByUserId { get; set; } = default!;
    }

}
