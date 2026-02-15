using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Comment;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.Comment.Queries.GetCommentsByPost
{
    public class GetCommentsByPostQuery : IRequest<Result<PagedResult<CommentDto>>>
    {
        public Guid PostId { get; set; }
        public Guid? ParentCommentId { get; set; } 
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
