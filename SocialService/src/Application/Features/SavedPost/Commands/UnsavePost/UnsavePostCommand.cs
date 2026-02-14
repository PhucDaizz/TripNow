using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.SavedPost.Commands.UnsavePost
{
    public class UnsavePostCommand : IRequest<Result<bool>>
    {
        public Guid PostId { get; set; } 
    }
}
