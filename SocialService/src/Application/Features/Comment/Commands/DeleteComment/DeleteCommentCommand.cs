using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.Comment.Commands.DeleteComment
{
    public class DeleteCommentCommand : IRequest<Result<bool>>
    {
        public Guid CommentId { get; set; }
    }
}
