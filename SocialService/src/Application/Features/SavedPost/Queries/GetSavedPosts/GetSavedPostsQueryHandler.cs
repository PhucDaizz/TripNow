using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.SavedPost;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.SavedPost.Queries.GetSavedPosts
{
    public class GetSavedPostsQueryHandler : IRequestHandler<GetSavedPostsQuery, Result<PagedResult<SavedPostDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetSavedPostsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedResult<SavedPostDto>>> Handle(GetSavedPostsQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            var query = _context.SavedPosts
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt); 

            var resultQuery = query.Join(_context.Posts,
                saved => saved.PostId,
                post => post.Id,
                (saved, post) => new SavedPostDto
                {
                    PostId = post.Id,
                    ContentShort = post.Content.Length > 100 ? post.Content.Substring(0, 100) + "..." : post.Content,
                    ThumbnailUrl = post.ThumbnailUrl,
                    SavedAt = saved.CreatedAt,
                    LikeCount = post.LikeCount,
                    CommentCount = post.CommentCount,
                });

            var totalCount = await resultQuery.CountAsync(cancellationToken);

            var items = await resultQuery
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<SavedPostDto>(items, totalCount, request.PageIndex, request.PageSize);

            return Result<PagedResult<SavedPostDto>>.Success(pagedResponse);
        }
    }
}
