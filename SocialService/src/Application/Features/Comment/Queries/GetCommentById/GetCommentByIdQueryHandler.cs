using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.Comment;

namespace SocialService.Application.Features.Comment.Queries.GetCommentById
{
    public class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, Result<CommentDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetCommentByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CommentDto>> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
        {
            
            var query = from c in _context.Comments.AsNoTracking()
                        join m in _context.Members.AsNoTracking()
                        on c.AuthorId equals m.Id into pm 
                        from author in pm.DefaultIfEmpty()
                        where c.Id == request.CommentId
                        select new CommentDto
                        {
                            Id = c.Id,
                            PostId = c.PostId,
                            UserId = c.AuthorId,
                            Content = c.Content,
                            ParentCommentId = c.ParentCommentId,
                            CreatedAt = c.CreatedAt,
                            UpdatedAt = c.UpdatedAt,

                            // Map thông tin User
                            UserName = author != null ? author.FullName : "Unknown User",
                            UserAvatar = author != null ? author.AvatarUrl : null
                        };

            var commentDto = await query.FirstOrDefaultAsync(cancellationToken);

            if (commentDto == null)
            {
                return Result.Failure<CommentDto>(new Error("NOT.FOUND", "Can not found this comment."));
            }

            return Result<CommentDto>.Success(commentDto);
        }
    }
}
