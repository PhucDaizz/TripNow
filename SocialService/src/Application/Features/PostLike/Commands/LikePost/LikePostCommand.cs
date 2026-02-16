using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.PostLike.Commands.LikePost
{
    public class LikePostCommand : IRequest<Result<bool>>
    {
        public Guid PostId { get; set; }
    }
}
