using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.Post;
using SocialService.Application.DTOs.Review;
using SocialService.Domain.Common.Models;

namespace SocialService.Application.Features.Post.Queries.GetPostsFeed
{
    public class GetPostsFeedQueryHandler : IRequestHandler<GetPostsFeedQuery, Result<PagedResult<PostDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetPostsFeedQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<PostDto>>> Handle(GetPostsFeedQuery request, CancellationToken cancellationToken)
        {
            var baseQuery = _context.Posts.AsNoTracking().Where(p => !p.IsDeleted);

            if (request.Type.HasValue)
            {
                baseQuery = baseQuery.Where(p => p.Type == request.Type.Value);
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return Result<PagedResult<PostDto>>.Success(
                    new PagedResult<PostDto>(new List<PostDto>(), request.PageIndex, request.PageSize, 0));
            }

            var query = from p in baseQuery
                        join m in _context.Members.AsNoTracking() on p.UserId equals m.Id into pm
                        from author in pm.DefaultIfEmpty()
                        orderby p.CreatedAt descending
                        select new PostDto
                        {
                            Id = p.Id,
                            HotelId = p.HotelId,
                            Content = p.Content,
                            Type = p.Type.ToString(),
                            LikeCount = p.LikeCount,
                            CommentCount = p.CommentCount,
                            CreatedAt = p.CreatedAt,
                            IsEdited = p.UpdatedAt.HasValue,

                            AuthorId = p.UserId,
                            AuthorName = author != null ? author.FullName : "Người dùng ẩn danh",
                            AuthorAvatar = author != null ? author.AvatarUrl : "",

                            Images = p.Images.Select(img => new PostImageDto
                            {
                                Id = img.Id,
                                ImageUrl = img.ImageUrl,
                                SortOrder = img.SortOrder,
                                Width = img.Width,
                                Height = img.Height,
                                Caption = img.Caption
                            }).OrderBy(img => img.SortOrder).ToList(),

                            Review = p.ReviewDetail != null ? new ReviewDetailDto
                            {
                                TargetId = p.ReviewDetail.TargetId,
                                TargetType = p.ReviewDetail.TargetType.ToString(),
                                Rating = p.ReviewDetail.Rating,
                                BookingId = p.ReviewDetail.BookingId
                            } : null
                        };

            var items = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<PostDto>(items, totalCount, request.PageIndex, request.PageSize);

            return Result<PagedResult<PostDto>>.Success(pagedResult);
        }
    }
}
