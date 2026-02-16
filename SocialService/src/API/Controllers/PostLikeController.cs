using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using SocialService.Application.DTOs.PostLike;
using SocialService.Application.Features.PostLike.Commands.LikePost;
using SocialService.Application.Features.PostLike.Commands.UnlikePost;
using SocialService.Application.Features.PostLike.Queries.GetPostLikes;
using SocialService.Domain.Common;

namespace SocialService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostLikeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostLikeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{postId}/like")]
        [Authorize(Roles = AppRoles.Customer)]
        public async Task<IActionResult> LikePost(Guid postId)
        {
            var result = await _mediator.Send(new LikePostCommand { PostId = postId });
            if (result.IsFailure) return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<bool>.SuccessResponse(true));
        }

        [HttpDelete("{postId}/like")]
        [Authorize(Roles = AppRoles.Customer)]
        public async Task<IActionResult> UnlikePost(Guid postId)
        {
            var result = await _mediator.Send(new UnlikePostCommand { PostId = postId });
            if (result.IsFailure) return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<bool>.SuccessResponse(true));
        }

        [HttpGet("{postId}/likes")]
        public async Task<IActionResult> GetLikes(Guid postId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
        {
            var query = new GetPostLikesQuery { PostId = postId, PageIndex = pageIndex, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<Domain.Common.Models.PagedResult<PostLikerDto>>.SuccessResponse(result.Value));
        }
    }
}
