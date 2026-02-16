using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.PostLike;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.PostLike.Queries.GetPostLikes
{
    public class GetPostLikesQuery: IRequest<Result<PagedResult<PostLikerDto>>>
    {
        public Guid PostId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
