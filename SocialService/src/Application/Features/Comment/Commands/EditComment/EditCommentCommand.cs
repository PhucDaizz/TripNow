using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.Comment.Commands.EditComment
{
    public class EditCommentCommand : IRequest<Result<bool>>
    {
        public Guid CommentId { get; set; }
        public string Content { get; set; }
    }
}
