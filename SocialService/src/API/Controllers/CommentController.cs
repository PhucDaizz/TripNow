using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using SocialService.Application.DTOs.Comment;
using SocialService.Application.Features.Comment.Commands.CreateComment;
using SocialService.Application.Features.Comment.Commands.DeleteComment;
using SocialService.Application.Features.Comment.Commands.EditComment;
using SocialService.Application.Features.Comment.Commands.HideComment;
using SocialService.Application.Features.Comment.Queries.GetCommentById;
using SocialService.Application.Features.Comment.Queries.GetCommentsByPost;
using SocialService.Domain.Common;

namespace SocialService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tạo mới comment
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCommentCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<Guid>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Sửa lại nội dung comment
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, [FromBody] EditCommentCommand command)
        {
            if (id != command.CommentId) return BadRequest("The ID in the URL and Body do not match");

            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<bool>.SuccessResponse(true));
        }

        /// <summary>
        /// Xoá nội dung comment
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteCommentCommand { CommentId = id };
            var result = await _mediator.Send(command);

            if (result.IsFailure) return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<bool>.SuccessResponse(true));
        }

        /// <summary>
        /// Lấy tất cả nội comments trong bài post
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetByPost(Guid postId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetCommentsByPostQuery
            {
                PostId = postId,
                ParentCommentId = null,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<Domain.Common.Models.PagedResult<CommentDto>>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Lấy phản hồi (trả lời) comment
        /// </summary>
        /// <remarks>
        /// - Dùng khi nhận được thông báo được phản hồi
        /// </remarks>
        [HttpGet("{commentId}/replies")]
        public async Task<IActionResult> GetReplies(Guid commentId, [FromQuery] Guid postId, [FromQuery] int pageIndex = 1)
        {
            var query = new GetCommentsByPostQuery
            {
                PostId = postId, 
                ParentCommentId = commentId, 
                PageIndex = pageIndex,
                PageSize = 5 
            };

            var result = await _mediator.Send(query);
            return Ok(ApiResponse<Domain.Common.Models.PagedResult<CommentDto>>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Ẩn comment
        /// </summary>
        /// <remarks>
        /// - Chỉ admin đươc phép ẩn của người khác
        /// - Ẩn trong trường hợp có hành vi tục tỉu, vi phạm, ...
        /// </remarks>
        [HttpPatch("{id}/hide")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> HideComment(Guid id, [FromBody] HideCommentCommand command)
        {
            if (id != command.CommentId)
                return BadRequest(ApiResponse<bool>.ErrorResponse("ID trên URL và Body không khớp"));

            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(ApiResponse<bool>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<bool>.SuccessResponse(true));
        }

        /// <summary>
        /// Lấy 1 comment
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCommentByIdQuery(id));

            if (result.IsFailure)
                return NotFound(ApiResponse<string>.ErrorResponse("Không tìm thấy bình luận"));

            return Ok(ApiResponse<CommentDto>.SuccessResponse(result.Value));
        }
    }
}
