using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.DTOs.Post;
using SocialService.Application.DTOs.Review;

namespace SocialService.Application.Features.Post.Queries.GetPostById
{
    public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, Result<PostDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPostByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PostDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var query = from p in _context.Posts.AsNoTracking()
                        join m in _context.Members.AsNoTracking() on p.UserId equals m.Id into pm
                        from author in pm.DefaultIfEmpty() 
                        where p.Id == request.PostId && !p.IsDeleted
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
                            AuthorType = p.AuthorType.ToString(),
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

            var postDto = await query.FirstOrDefaultAsync(cancellationToken);

            if (postDto == null)
            {
                return Result.Failure<PostDto>(new Error("NOT_FOUND", "The post does not exist or has been deleted."));
            }

            return Result<PostDto>.Success(postDto);
        }
    }
}
