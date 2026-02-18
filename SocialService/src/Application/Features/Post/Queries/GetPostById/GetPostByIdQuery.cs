using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Post;

namespace SocialService.Application.Features.Post.Queries.GetPostById
{
    public class GetPostByIdQuery : IRequest<Result<PostDto>>
    {
        public Guid PostId { get; set; }
    }
}
