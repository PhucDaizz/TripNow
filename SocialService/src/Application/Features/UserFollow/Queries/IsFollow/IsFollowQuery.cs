using Domain.Common.Response;
using MediatR;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.UserFollow.Queries.IsFollow
{
    public class IsFollowQuery: IRequest<Result<bool>>
    {
        public Guid TargetId { get; set; }
        public TypeFollow TypeFollow { get; set; }
    }
}
