using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.Comment;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.Comment.Queries.GetCommentsByPost
{
    public class GetCommentsByPostQueryHandler : IRequestHandler<GetCommentsByPostQuery, Result<PagedResult<CommentDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetCommentsByPostQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<CommentDto>>> Handle(GetCommentsByPostQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Comments.AsNoTracking()
                .Where(c => c.PostId == request.PostId && !c.IsDeleted && !c.IsHidden);

            if (request.ParentCommentId.HasValue)
            {
                query = query.Where(c => c.ParentCommentId == request.ParentCommentId);
            }
            else
            {
                query = query.Where(c => c.ParentCommentId == null);
            }

            var resultQuery = from c in query
                              join u in _context.Members on c.AuthorId equals u.Id
                              orderby c.CreatedAt descending
                              select new CommentDto
                              {
                                  Id = c.Id,
                                  PostId = c.PostId,
                                  UserId = c.AuthorId,
                                  UserName = u.FullName, 
                                  UserAvatar = u.AvatarUrl,
                                  Content = c.Content,
                                  ParentCommentId = c.ParentCommentId,
                                  CreatedAt = c.CreatedAt,
                                  UpdatedAt = c.UpdatedAt,
                                  ReplyCount = _context.Comments.Count(reply =>
                                      reply.ParentCommentId == c.Id &&
                                      !reply.IsDeleted &&
                                      !reply.IsHidden)
                              };

            var totalCount = await resultQuery.CountAsync(cancellationToken);
            var items = await resultQuery
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return Result<PagedResult<CommentDto>>.Success(
                new PagedResult<CommentDto>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
