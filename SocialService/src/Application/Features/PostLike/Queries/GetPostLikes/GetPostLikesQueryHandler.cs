using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.PostLike;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.PostLike.Queries.GetPostLikes
{
    public class GetPostLikesQueryHandler : IRequestHandler<GetPostLikesQuery, Result<PagedResult<PostLikerDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetPostLikesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<PostLikerDto>>> Handle(GetPostLikesQuery request, CancellationToken cancellationToken)
        {
            var query = from like in _context.PostLikes.AsNoTracking()
                        join member in _context.Members.AsNoTracking() on like.UserId equals member.Id
                        where like.PostId == request.PostId
                        orderby like.CreatedAt descending
                        select new PostLikerDto
                        {
                            UserId = like.UserId,
                            FullName = member.FullName,
                            AvatarUrl = member.AvatarUrl,
                            LikedAt = like.CreatedAt
                        };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<PostLikerDto>(items, totalCount, request.PageIndex, request.PageSize);

            return Result<PagedResult<PostLikerDto>>.Success(pagedResult);
        }
    }
}