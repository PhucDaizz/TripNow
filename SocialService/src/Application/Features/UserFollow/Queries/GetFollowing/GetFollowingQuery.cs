using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.UserFollow;
using SocialService.Domain.Common.Models;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.UserFollow.Queries.GetFollowing
{
    public class GetFollowingQuery : IRequest<Result<PagedResult<FollowItemDto>>>
    {
        public Guid UserId { get; set; }
        public TypeFollow? FilterType { get; set; } 
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
