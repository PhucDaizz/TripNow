using Application.Common.Interfaces;
using Application.DTOs.User;
using Application.Features.User.Commands.UpdateInfor;
using Application.Features.User.Commands.UploadAvatar;
using Application.Features.User.Queries.GetInfoDetail;
using Application.Features.User.Queries.GetUsersWithPagination;
using Application.Features.User.Queries.IsUserExisting;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public UserController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Xem thông tin của bản thân
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<GetInfoDetailQuery>.ErrorResponse("User not authenticated"));
            }

            var result = await _mediator.Send(new GetInfoDetailQuery
            {
                UserId = userId
            });

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<InforDto>.SuccessResponse(result.Value));
            }
            return BadRequest(ApiResponse<InforDto>.ErrorResponse(
                result.Error.Code,
                new List<string> { result.Error.Message }
            ));
        }

        /// <summary>
        /// Cập nhật thông tin cá nhân
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateInfor([FromBody]UpdateInforCommand command)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("User not found!"));
            }

            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<string>.SuccessResponse(result.Value, "User information updated successfully."));
            }
            return BadRequest(ApiResponse<string>.ErrorResponse(
                result.Error.Code,
                new List<string> { result.Error.Message }
            ));

        }

        /// <summary>
        /// Tải lên ảnh đại diện lưu ý dùng IFormFile
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPost("upload-avatar")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB
        [RequestFormLimits(MultipartBodyLengthLimit = 5 * 1024 * 1024)]
        public async Task<IActionResult> UploadAvatar(IFormFile file, [FromQuery] bool deleteOld = true)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var result = await _mediator.Send(new UploadAvatarCommand
                {
                    File = file,
                    UserId = userId,
                    DeleteOldAvatar = deleteOld,
                    OptimizeImage = true
                });

                return Ok(ApiResponse<UploadAvatarResult>.SuccessResponse(result.Value, "Avatar uploaded successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("UPLOAD_FAILED", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Admin xem thông tin người dùng
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("{userId}")]
        [Authorize(Roles = AppRoles.SysAdmin)]
        public async Task<IActionResult> GetUserInfo(string userId)
        {
            var result = await _mediator.Send(new GetInfoDetailQuery
            {
                UserId = userId
            });

            return result.IsSuccess
                ? Ok(ApiResponse<InforDto>.SuccessResponse(result.Value))
                : NotFound(ApiResponse<InforDto>.ErrorResponse(result.Error.Code));
        }

        /// <summary>
        /// Xem danh sách người dùng
        /// </summary>
        /// <remarks>
        /// - Admin xem được tất cả 
        /// - Chủ khách sạn chỉ xem nhân viên của khách sạn đó
        /// - Lễ tân chỉ xem nhân viên của khách sạn đó
        /// - Người dùng thông thường không được phép
        /// Trường Role có các role gồm "SysAdmin", "HotelOwner", "Receptionist", "Housekeeping", "Customer"
        /// </remarks>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersWithPaginationQuery query)
        {
            var currentRole = User.FindFirstValue(ClaimTypes.Role);
            var currentHotelId = _currentUserService.HotelId;

            if (currentRole == AppRoles.SysAdmin)
            {
            }
            else if (currentRole == AppRoles.HotelOwner)
            {
                if (currentHotelId == null)
                {
                    return Ok(ApiResponse<PagedResult<UserDto>>.SuccessResponse(PagedResult<UserDto>.Empty()));
                }

                query.HotelId = currentHotelId;
            }
            else if (currentRole == AppRoles.Receptionist)
            {
                if (currentHotelId == null)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Staff account implies a hotel association but none found."));
                }
                query.HotelId = currentHotelId;
            }
            else
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
            }

            return Ok(ApiResponse<Domain.Common.Models.PagedResult<UserDto>>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Không map ra APIGateWay
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("user-existing")]
        public async Task<IActionResult> IsUsserExisting([FromQuery]Guid userId)
        {
            var request = new IsUserExistingQuery
            {
                UserId = userId
            };

            var result = await _mediator.Send(request);
            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
        }
    }
}
