using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.Post.Commands.DeletePost
{
    public class DeletePostCommand : IRequest<Result<bool>>
    {
        public Guid PostId { get; set; }
    }
}
