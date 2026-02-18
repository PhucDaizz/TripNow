using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Post;
using SocialService.Domain.Common.Models;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Post.Queries.GetPostsFeed
{
    public class GetPostsFeedQuery : IRequest<Result<PagedResult<PostDto>>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public PostType? Type { get; set; } 
    }
}
