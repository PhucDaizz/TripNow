using Domain.Common.Response;
using MediatR;

namespace Application.Features.StaffProfile.Commands.DeleteStaffProfile
{
    public class DeleteStaffProfileCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public string DeletedByUserId { get; set; } = default!;
    }
}
