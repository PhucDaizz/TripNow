using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using SocialService.Application.Features.UserFollow.Commands.FollowHotel;
using SocialService.Application.Features.UserFollow.Commands.FollowUser;
using SocialService.Application.Features.UserFollow.Commands.UnfollowHotel;
using SocialService.Application.Features.UserFollow.Commands.UnfollowUser;
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

        [HttpPost("user")]
        [Authorize($"{AppRoles.Customer}")]
        public async Task<IActionResult> FollowUser([FromBody] FollowUserCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }

        [HttpPost("hotel")]
        [Authorize($"{AppRoles.Customer}")]
        public async Task<IActionResult> FollowHotel([FromBody] FollowHotelCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }

        [HttpGet("check/{targetId}")]
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

    }
}
