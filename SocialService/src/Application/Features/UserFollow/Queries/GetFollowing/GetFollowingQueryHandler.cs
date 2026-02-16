using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.UserFollow;
using SocialService.Domain.Common;
using SocialService.Domain.Common.Models;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.UserFollow.Queries.GetFollowing
{
    public class GetFollowingQueryHandler : IRequestHandler<GetFollowingQuery, Result<PagedResult<FollowItemDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService; 

        public GetFollowingQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedResult<FollowItemDto>>> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId);
            var userRoles = _currentUserService.Role; 
            var isAdmin = userRoles.Contains(AppRoles.SysAdmin);

            if (!isAdmin && currentUserId != request.UserId)
            {
                return Result.Failure<PagedResult<FollowItemDto>>(new Error("NOT.PERMIT", "You do not have permission to view other people's following lists."));
            }

            var query = _context.UserFollows.AsNoTracking()
                .Where(x => x.FollowerId == request.UserId); 

            if (request.FilterType.HasValue)
            {
                query = query.Where(x => x.Type == request.FilterType.Value);
            }


            var totalCount = await query.CountAsync(cancellationToken);

            var rawItems = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new
                {
                    x.TargetId,
                    x.Type,
                    x.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var userIds = rawItems.Where(x => x.Type == TypeFollow.FollowUser).Select(x => x.TargetId).ToList();
            var hotelIds = rawItems.Where(x => x.Type == TypeFollow.FollowHotel).Select(x => x.TargetId).ToList();

            var usersInfo = await _context.Members.Where(m => userIds.Contains(m.Id)).ToDictionaryAsync(k => k.Id, v => new { v.FullName, v.AvatarUrl });

            var resultItems = rawItems.Select(item =>
            {
                string name = "Unknown";
                string avatar = "";

                if (item.Type == TypeFollow.FollowUser)
                {
                    if (usersInfo.TryGetValue(item.TargetId, out var u))
                    {
                        name = u.FullName;
                        avatar = u.AvatarUrl;
                    }
                }
                else if (item.Type == TypeFollow.FollowHotel)
                {
                    name = "Khách sạn";
                }

                return new FollowItemDto
                {
                    TargetId = item.TargetId,
                    TargetName = name,
                    TargetAvatar = avatar,
                    Type = item.Type.ToString(),
                    FollowedAt = item.CreatedAt
                };
            }).ToList();

            return Result<PagedResult<FollowItemDto>>.Success(
                new PagedResult<FollowItemDto>(resultItems, request.PageIndex, request.PageSize, totalCount));
        }
    }
}
