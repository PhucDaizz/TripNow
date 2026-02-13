using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.UserFollow.Commands.FollowUser
{
    public class FollowUserCommand: IRequest<Result<bool>>
    {
        public Guid TargetId { get; set; }
    }
}
