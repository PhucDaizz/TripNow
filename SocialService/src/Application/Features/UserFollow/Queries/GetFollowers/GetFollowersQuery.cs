using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.UserFollow;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.UserFollow.Queries.GetFollowers
{
    public class GetFollowersQuery : IRequest<Result<PagedResult<FollowerDto>>>
    {
        public Guid UserId { get; set; } 
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
