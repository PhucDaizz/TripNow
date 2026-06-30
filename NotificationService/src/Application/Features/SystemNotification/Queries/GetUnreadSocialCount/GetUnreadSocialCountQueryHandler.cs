using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Common.Interfaces;

namespace NotificationService.Application.Features.SystemNotification.Queries.GetUnreadSocialCount
{
    public class GetUnreadSocialCountQueryHandler : IRequestHandler<GetUnreadSocialCountQuery, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public GetUnreadSocialCountQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(GetUnreadSocialCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _context.SocialNotificationsQuery
                .AsNoTracking()
                .Where(x => x.OwnerId == request.UserId && x.IsRead == false)
                .CountAsync();

            return Result<int>.Success(count);
        }
    }
}
