using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.SavedPost.Commands.SavePost
{
    public class SavePostCommand : IRequest<Result<bool>>
    {
        public Guid PostId { get; set; }
    }
}
