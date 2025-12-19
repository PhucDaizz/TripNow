using Application.DTOs.StaffProfile;
using Domain.Common.Response;
using MediatR;

namespace Application.Features.StaffProfile.Commands.CreateStaffProfile
{
    public class CreateStaffProfileCommand : IRequest<Result<StaffProfileDto>>
    {
        public string Email { get; set; }
        public Guid HotelId { get; set; }
        public string Position { get; set; }
        public string CreatedByUserId { get; set; } = default!;
    }
}
