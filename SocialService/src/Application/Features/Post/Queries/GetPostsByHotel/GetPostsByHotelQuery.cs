using Domain.Common.Response;
using MediatR;
using SocialService.Application.DTOs.Post;
using SocialService.Domain.Common.Models;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Post.Queries.GetPostsByHotel
{
    public class GetPostsByHotelQuery : IRequest<Result<PagedResult<PostDto>>>
    {
        public Guid HotelId { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public PostType? Type { get; set; }
        public PostAuthorType? AuthorType { get; set; }
    }
}
