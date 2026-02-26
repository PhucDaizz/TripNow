using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.UserFollow;
using SocialService.Domain.Common;
using SocialService.Domain.Common.Models;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.UserFollow.Queries.GetFollowers
{
    public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, Result<PagedResult<FollowerDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetFollowersQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedResult<FollowerDto>>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId);
            var userRoles = _currentUserService.Role;
            var isAdmin = userRoles.Contains(AppRoles.SysAdmin);

            if (!isAdmin && currentUserId != request.UserId)
            {
                return Result.Failure<PagedResult<FollowerDto>>(new Error("NOT.PERMIT", "You do not have permission to view other people's follower lists."));
            }

            var query = _context.UserFollows.AsNoTracking()
                .Where(x => x.TargetId == request.UserId && x.Type == TypeFollow.FollowUser);

            var resultQuery = from f in query
                              join m in _context.Members.AsNoTracking()
                              on f.FollowerId equals m.Id
                              orderby f.CreatedAt descending
                              select new FollowerDto
                              {
                                  FollowerId = f.FollowerId,
                                  FollowerName = m.FullName,
                                  FollowerAvatar = m.AvatarUrl,
                                  FollowedAt = f.CreatedAt
                              };

            var totalCount = await resultQuery.CountAsync(cancellationToken);

            var items = await resultQuery
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return Result<PagedResult<FollowerDto>>.Success(
                new PagedResult<FollowerDto>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
