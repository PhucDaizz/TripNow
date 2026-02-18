using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Common;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Post.Commands.CreateReviewPost
{
    public class CreateReviewPostCommand : IRequest<Result<Guid>>
    {
        public Guid HotelId { get; set; }
        public string Content { get; set; }

        public TargetTypeReview TargetType { get; set; }
        public Guid TargetId { get; set; }
        public decimal Rating { get; set; } 
        public Guid? BookingId { get; set; }

        public List<FileDto>? Images { get; set; }
        public List<string>? ImageCaptions { get; set; }
    }
}
