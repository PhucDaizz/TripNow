using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.Comment.Commands.CreateComment
{
    public class CreateCommentCommand : IRequest<Result<Guid>>
    {
        public Guid PostId { get; set; }
        public string Content { get; set; }
        public Guid? ParentCommentId { get; set; } 
    }
}
