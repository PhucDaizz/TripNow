using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Common.Interfaces;

namespace NotificationService.Application.Features.SocialNotification.Queries.GetUnreadSystemCount
{
    public class GetUnreadSystemCountQueryHandler : IRequestHandler<GetUnreadSystemCountQuery, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public GetUnreadSystemCountQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(GetUnreadSystemCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _context.Notifications
                .Where(x => x.UserId == request.UserId && x.IsRead == false) 
                .CountAsync(cancellationToken);

            return Result<int>.Success(count);
        }
    }
}
