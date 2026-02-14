using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.SavedPost;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.SavedPost.Queries.GetSavedPosts
{
    public class GetSavedPostsQuery : IRequest<Result<PagedResult<SavedPostDto>>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
