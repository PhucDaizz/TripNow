using Application.DTOs.StaffProfile;
using Domain.Common.Response;
using MediatR;

namespace Application.Features.StaffProfile.Queries.GetStaffProfile
{
    public class GetStaffProfileQuery: IRequest<Result<StaffProfileDto>>
    {
        public string UserId { get; init; }
    }
}
