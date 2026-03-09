using Domain.Common.Response;
using MediatR;

namespace NotificationService.Application.Features.SocialNotification.Queries.GetUnreadSystemCount
{
    public record GetUnreadSystemCountQuery(Guid UserId): IRequest<Result<int>>;
}
s