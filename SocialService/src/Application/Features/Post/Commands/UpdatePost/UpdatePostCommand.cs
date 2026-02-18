using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Common;

namespace SocialService.Application.Features.Post.Commands.UpdatePost
{
    public class UpdatePostCommand : IRequest<Result<bool>>
    {
        public Guid PostId { get; set; }
        public string Content { get; set; }
        public List<Guid>? DeletedImageIds { get; set; }
        public List<FileDto>? NewImages { get; set; }
        public List<string>? NewImageCaptions { get; set; }
    }
}
