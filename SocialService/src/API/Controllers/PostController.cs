using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using SocialService.API.Models.Post;
using SocialService.Application.DTOs.Common;
using SocialService.Application.DTOs.Post;
using SocialService.Application.Features.Post.Commands.CreateEventPost;
using SocialService.Application.Features.Post.Commands.CreateNormalPost;
using SocialService.Application.Features.Post.Commands.CreateReviewPost;
using SocialService.Application.Features.Post.Commands.DeletePost;
using SocialService.Application.Features.Post.Commands.UpdatePost;
using SocialService.Application.Features.Post.Queries.GetPostById;
using SocialService.Application.Features.Post.Queries.GetPostsByHotel;
using SocialService.Application.Features.Post.Queries.GetPostsByUser;
using SocialService.Application.Features.Post.Queries.GetPostsFeed;
using SocialService.Domain.Common;
using SocialService.Domain.Enum;

namespace SocialService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tạo 1 bài post thông thường
        /// </summary>
        /// <remarks>
        /// - Ví dụ có thể là 1 bài post "hằng ngày, vui chơi,..."
        /// </remarks>
        [HttpPost("normal")]
        [Authorize]
        public async Task<IActionResult> CreateNormalPost([FromForm] CreateNormalPostRequest request)
        {
            var command = new CreateNormalPostCommand
            {
                HotelId = request.HotelId,
                Content = request.Content,
                ImageCaptions = request.ImageCaptions,
                Images = new List<FileDto>()
            };

            if (request.Images != null)
            {
                foreach (var file in request.Images)
                {
                    if (file.Length > 0)
                    {
                        var fileDto = new FileDto
                        {
                            FileName = file.FileName,
                            ContentType = file.ContentType,
                            Length = file.Length,
                            Content = file.OpenReadStream() 
                        };
                        command.Images.Add(fileDto);
                    }
                }
            }

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Tạo 1 bài post review
        /// </summary>
        /// <remarks>
        /// - Ví dụ có thể là 1 bài post sau khi trải nghiệm xong 1 dịch vụ khách sạn hoặc trải nghiệm tại 1 địa điểm được đê xuất trên Location
        /// </remarks>
        [HttpPost("review")]
        [Authorize(Roles = AppRoles.Customer)] 
        public async Task<IActionResult> CreateReviewPost([FromForm] CreateReviewPostRequest request)
        {
            var command = new CreateReviewPostCommand
            {
                HotelId = request.HotelId,
                Content = request.Content,
                TargetType = request.TargetType,
                TargetId = request.TargetId,
                Rating = request.Rating,
                BookingId = request.BookingId,
                ImageCaptions = request.ImageCaptions,
                Images = new List<FileDto>()
            };

            if (request.Images != null)
            {
                foreach (var file in request.Images)
                {
                    if (file.Length > 0)
                    {
                        var fileDto = new FileDto
                        {
                            FileName = file.FileName,
                            ContentType = file.ContentType,
                            Length = file.Length,
                            Content = file.OpenReadStream() 
                        };
                        command.Images.Add(fileDto);
                    }
                }
            }

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.ToString()));
            }

            return Ok(ApiResponse<Guid>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Tạo 1 bài post sự kiên cho khách sạn
        /// </summary>
        /// <remarks>
        /// - Chỉ chủ khách sạn được tạo
        /// - Post này có thê post nhằm mục đích giảm giá hoặc thông báo nghĩ lễ ,....
        /// </remarks>
        [HttpPost("event")]
        [Authorize(Roles = AppRoles.HotelOwner)] 
        public async Task<IActionResult> CreateEventPost([FromForm] CreateEventPostRequest request)
        {
            var command = new CreateEventPostCommand
            {
                HotelId = request.HotelId,
                Content = request.Content,
                ImageCaptions = request.ImageCaptions,
                Images = new List<FileDto>()
            };

            if (request.Images != null)
            {
                foreach (var file in request.Images)
                {
                    if (file.Length > 0)
                    {
                        var fileDto = new FileDto
                        {
                            FileName = file.FileName,
                            ContentType = file.ContentType,
                            Length = file.Length,
                            Content = file.OpenReadStream()
                        };
                        command.Images.Add(fileDto);
                    }
                }
            }

            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<Guid>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Sửa lại 1 bài post
        /// </summary>
        /// <remarks>
        /// - Tất cả người đăng nhập đều có thể sửa miễn là bài viết của họ
        /// </remarks>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(Guid id, [FromForm] UpdatePostRequest request)
        {
            var command = new UpdatePostCommand
            {
                PostId = id,
                Content = request.Content,
                DeletedImageIds = request.DeletedImageIds, 
                NewImages = new List<FileDto>(),
                NewImageCaptions = request.NewImageCaptions
            };

            if (request.NewImages != null && request.NewImages.Any())
            {
                foreach (var file in request.NewImages)
                {
                    if (file.Length > 0)
                    {
                        var fileDto = new FileDto
                        {
                            FileName = file.FileName,
                            ContentType = file.ContentType,
                            Length = file.Length,
                            Content = file.OpenReadStream() 
                        };
                        command.NewImages.Add(fileDto);
                    }
                }
            }

            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<bool>.SuccessResponse(true));
        }

        /// <summary>
        /// Xoá lại 1 bài post
        /// </summary>
        /// <remarks>
        /// - Tất cả người đăng nhập đều có thể xoá miễn là bài viết của họ
        /// - Admin được quyền xoá tất cả
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var result = await _mediator.Send(new DeletePostCommand { PostId = id });

            if (result.IsFailure) return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<bool>.SuccessResponse(true));
        }

        /// <summary>
        /// Lất 1 bài post
        /// </summary>
        /// <remarks>
        /// - Khi người dùng bấm vào một thông báo, hoặc copy link bài viết gửi cho người khác
        /// </remarks>
        [HttpGet("{id}")]
        [AllowAnonymous]  
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var query = new GetPostByIdQuery { PostId = id };
            var result = await _mediator.Send(query);

            if (result.IsFailure) return NotFound(ApiResponse<string>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<PostDto>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Load danh sách bài viết ở màn hình chính, có phân trang, sắp xếp bài mới nhất lên đầu, và có thể filter (chỉ xem bài Review, hoặc xem tất cả).
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet] // 
        public async Task<IActionResult> GetPostsFeed([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] PostType? type = null)
        {
            var query = new GetPostsFeedQuery
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Type = type
            };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<Domain.Common.Models.PagedResult<PostDto>>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Load danh sách bài viết (Event, Review, Normal check-in) thuộc về khách sạn đó.
        /// </summary>
        /// <remarks>
        /// - Khi user bấm vào trang Profile của 1 khách sạn, họ sẽ muốn xem tất cả các bài Post (Event, Review, Normal check-in) thuộc về khách sạn đó.
        /// </remarks>
        [HttpGet("hotel/{hotelId}")] 
        public async Task<IActionResult> GetPostsByHotel(Guid hotelId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetPostsByHotelQuery
            {
                HotelId = hotelId,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<Domain.Common.Models.PagedResult<PostDto>>.SuccessResponse(result.Value));
        }

        /// <summary>
        ///  Load danh sách bài viết (Event, Review, Normal check-in) thuộc về người dùng đó
        /// </summary>
        /// <remarks>
        /// // Vào trang cá nhân của 1 người dùng (Profile), tải danh sách các bài họ đã đăng.
        /// </remarks>
        [HttpGet("user/{userId}")] 
        public async Task<IActionResult> GetPostsByUser(Guid userId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetPostsByUserQuery
            {
                UserId = userId,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<Domain.Common.Models.PagedResult<PostDto>>.SuccessResponse(result.Value));
        }
    }

}
