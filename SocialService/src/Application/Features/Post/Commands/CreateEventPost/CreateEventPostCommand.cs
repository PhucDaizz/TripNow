using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Common;

namespace SocialService.Application.Features.Post.Commands.CreateEventPost
{
    public class CreateEventPostCommand : IRequest<Result<Guid>>
    {
        public Guid HotelId { get; set; }
        public string Content { get; set; }
        public List<FileDto>? Images { get; set; }
        public List<string>? ImageCaptions { get; set; }
    }
}
