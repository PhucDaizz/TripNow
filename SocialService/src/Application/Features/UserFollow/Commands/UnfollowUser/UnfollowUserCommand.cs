using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.UserFollow.Commands.UnfollowUser
{
    public class UnfollowUserCommand: IRequest<Result<bool>>
    {
        public Guid UserTargetId { get; set; }
    }
}
