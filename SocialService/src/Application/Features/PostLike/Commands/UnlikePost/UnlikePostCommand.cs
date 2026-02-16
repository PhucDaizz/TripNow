using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.PostLike.Commands.UnlikePost
{
    public class UnlikePostCommand : IRequest<Result<bool>>
    {
        public Guid PostId { get; set; }
    }
}
