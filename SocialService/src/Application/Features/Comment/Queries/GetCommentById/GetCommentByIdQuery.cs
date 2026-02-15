using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Comment;

namespace SocialService.Application.Features.Comment.Queries.GetCommentById
{
    public record GetCommentByIdQuery(Guid CommentId) : IRequest<Result<CommentDto>>;
}
