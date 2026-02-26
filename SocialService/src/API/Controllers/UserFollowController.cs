using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using SocialService.Application.DTOs.UserFollow;
using SocialService.Application.Features.UserFollow.Commands.FollowHotel;
using SocialService.Application.Features.UserFollow.Commands.FollowUser;
using SocialService.Application.Features.UserFollow.Commands.UnfollowHotel;
using SocialService.Application.Features.UserFollow.Commands.UnfollowUser;
using SocialService.Application.Features.UserFollow.Queries.GetFollowers;
using SocialService.Application.Features.UserFollow.Queries.GetFollowing;
using SocialService.Application.Features.UserFollow.Queries.IsFollow;
using SocialService.Domain.Common;
using SocialService.Domain.Enum;

namespace SocialService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserFollowController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserFollowController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Follow người dùng
        /// </summary>
        /// <remarks>
        /// - Chỉ người dùng sử dụng được
        /// </remarks>
        [HttpPost("user")]
        [Authorize(Roles = $"{AppRoles.Customer}")]
        public async Task<IActionResult> FollowUser([FromBody] FollowUserCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Follow khách sạn
        /// </summary>
        /// <remarks>
        /// - Chỉ người dùng sử dụng được
        /// </remarks>
        [HttpPost("hotel")]
        [Authorize(Roles = $"{AppRoles.Customer}")]
        public async Task<IActionResult> FollowHotel([FromBody] FollowHotelCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Kiểm tra người dùng đã theo dõi đối phương hay chưa
        /// </summary>
        /// <remarks>
        /// - Tất cả role đều dùng được
        /// </remarks>
        [HttpGet("check/{targetId}")]
        [Authorize]
        public async Task<IActionResult> IsFollow(Guid targetId, [FromQuery]TypeFollow typeFollow)
        {
            var request = new IsFollowQuery
            {
                TargetId = targetId,
                TypeFollow = typeFollow
            };

            var reponse = await _mediator.Send(request);

            return Ok(ApiResponse<bool>.SuccessResponse(reponse.Value));
        }

        /// <summary>
        /// Huỷ follow người dùng
        /// </summary>
        /// <remarks>
        /// - Chỉ người dùng sử dụng được
        /// </remarks>
        [HttpDelete("user/{targetId}")] 
        [Authorize(Roles = AppRoles.Customer)]
        public async Task<IActionResult> UnfollowUser(Guid targetId)
        {
            var command = new UnfollowUserCommand { UserTargetId = targetId };

            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Huỷ follow khách sạn
        /// </summary>
        /// <remarks>
        /// - Chỉ người dùng sử dụng được
        /// </remarks>
        [HttpDelete("hotel/{hotelId}")] 
        [Authorize(Roles = AppRoles.Customer)]
        public async Task<IActionResult> UnfollowHotel(Guid hotelId)
        {
            var command = new UnfollowHotelCommand { HotelId = hotelId };

            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Danh sách những người/khách sạn mà User đang theo dõi.
        /// </summary>
        /// <remarks>
        /// - Admin xem được tất cả người dùng chỉ xem của họ
        /// </remarks>
        [HttpGet("{userId}/following")]
        [Authorize] 
        public async Task<IActionResult> GetFollowingList(
            Guid userId,
            [FromQuery] TypeFollow? type,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetFollowingQuery
            {
                UserId = userId,
                FilterType = type,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                if (result.Error.ToString().Contains("do not have permission"))
                    return StatusCode(403, ApiResponse<string>.ErrorResponse(result.Error.ToString()));

                return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.ToString()));
            }

            return Ok(ApiResponse<Domain.Common.Models.PagedResult<FollowItemDto>>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Danh sách những người đang theo dõi User này.
        /// </summary>
        /// <remarks>
        /// - Admin xem được tất cả người dùng chỉ xem của họ
        /// </remarks>
        [HttpGet("{userId}/followers")]
        [Authorize] 
        public async Task<IActionResult> GetFollowersList(
            Guid userId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetFollowersQuery
            {
                UserId = userId,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                if (result.Error.ToString().Contains("do not have permission"))
                    return StatusCode(403, ApiResponse<string>.ErrorResponse(result.Error.ToString()));

                return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.ToString()));
            }

            return Ok(ApiResponse<Domain.Common.Models.PagedResult<FollowerDto>>.SuccessResponse(result.Value));
        }

    }
}
