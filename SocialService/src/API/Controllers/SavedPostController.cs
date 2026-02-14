using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using SocialService.Application.DTOs.SavedPost;
using SocialService.Application.Features.SavedPost.Commands.SavePost;
using SocialService.Application.Features.SavedPost.Commands.UnsavePost;
using SocialService.Application.Features.SavedPost.Queries.GetSavedPosts;
using SocialService.Domain.Common;

namespace SocialService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Customer)]
    public class SavedPostController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SavedPostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> SavePost([FromBody] SavePostCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> UnsavePost(Guid postId)
        {
            var command = new UnsavePostCommand { PostId = postId };
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }

        [HttpGet]
        public async Task<IActionResult> GetMySavedPosts([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetSavedPostsQuery { PageIndex = pageIndex, PageSize = pageSize };
            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<Domain.Common.Models.PagedResult<SavedPostDto>>.SuccessResponse(result.Value));
        }
    }
}
