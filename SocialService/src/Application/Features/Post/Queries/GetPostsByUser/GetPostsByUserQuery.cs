using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Post;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.Post.Queries.GetPostsByUser
{
    public class GetPostsByUserQuery : IRequest<Result<PagedResult<PostDto>>>
    {
        public Guid UserId { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
