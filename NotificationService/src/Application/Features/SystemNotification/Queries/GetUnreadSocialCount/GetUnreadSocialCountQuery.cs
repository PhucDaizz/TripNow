using Domain.Common.Response;
using MediatR;

namespace NotificationService.Application.Features.SystemNotification.Queries.GetUnreadSocialCount
{
    public record GetUnreadSocialCountQuery(Guid UserId) : IRequest<Result<int>>;
}
