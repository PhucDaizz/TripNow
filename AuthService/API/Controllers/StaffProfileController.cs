using Application.DTOs.StaffProfile;
using Application.Features.StaffProfile.Commands.CreateStaffProfile;
using Application.Features.StaffProfile.Commands.DeleteStaffProfile;
using Application.Features.StaffProfile.Commands.UpdateStaffProfile;
using Application.Features.StaffProfile.Queries.GetStaffProfile;
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
    public class StaffProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StaffProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Thêm nhân viên vào hệ thống khách sạn 
        /// </summary>
        /// <remarks>
        /// Lưu ý: 1.chỉ admin hoặc chủ khách sạn đó mới được thêm
        /// 2. Người muốn làm nhân viên phải đăng ký tài khoản khách hàng trước đó
        /// Email là email của người muốn thêm
        /// Các vị trí được thêm gồm: "HotelOwner", "Receptionist", "Housekeeping"
        /// </remarks>
        [HttpPost("staff-profile")]
        [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.HotelOwner}")]
        public async Task<IActionResult> CreateStaffProfile(
            [FromBody]CreateStaffProfileDto command)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newStaffProfile = new CreateStaffProfileCommand
            {
                Email = command.Email,
                HotelId = command.HotelId,
                Position = command.Position,
                CreatedByUserId = currentUserId
            };

            var result = await _mediator.Send(newStaffProfile);

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<StaffProfileDto>.SuccessResponse(
                    result.Value,
                    "Staff profile created successfully"
                ));
            }

            return BadRequest(ApiResponse<string>.ErrorResponse(
                result.Error.Message,
                new List<string> { result.Error.Code }
            ));
        }

        /// <summary>
        /// Xoá nhân viên ra khỏi hệ thống khách sạn
        /// </summary>
        /// <remarks>
        /// Sau khi xoá sẽ được trở về role khách hàng thông thường
        /// Lưu ý chỉ amdin hoặc chủ khách sạn mới được xoá
        /// </remarks>
        [HttpDelete("staff/{userid}")]
        [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.HotelOwner}")]
        public async Task<IActionResult> DeleteStaffProfile(Guid userid)
        {
            var command = new DeleteStaffProfileCommand
            {
                UserId = userid,
                DeletedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return NoContent();

            return BadRequest(result.Error.Message);
        }

        /// <summary>
        /// Cập nhật lại chức vụ của nhân viên
        /// </summary>
        /// <remarks>
        /// Lưu ý: chỉ admin và chủ khách sạn mới được cập nhật
        /// Các role mới được cập nhật gồm "HotelOwner", "Receptionist", "Housekeeping"
        /// </remarks>
        [HttpPut("staff/{userId}")]
        [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.HotelOwner}")]
        public async Task<IActionResult> UpdateStaffProfile(
            Guid userId,
            [FromBody] UpdateStaffProfileDto command)
        {

            var staffProfileUpdate = new UpdateStaffProfileCommand
            {
                StaffProfileId = userId,
                NewPosition = command.NewPosition,
                HotelId = command.HotelId,
                UpdatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };

            var result = await _mediator.Send(staffProfileUpdate);

            if (result.IsSuccess)
                return Ok(ApiResponse<StaffProfileDto>.SuccessResponse(result.Value));

            return BadRequest(ApiResponse<StaffProfileDto>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Xem thông tin nhân viên của khách sạn nào role khách hàng không được xem
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("staff-profile")]
        [Authorize(Roles = $"{AppRoles.SysAdmin},{AppRoles.HotelOwner},{AppRoles.Housekeeping},{AppRoles.Receptionist}")]
        public async Task<IActionResult> GetStaffProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(new GetStaffProfileQuery { UserId = userId });

            if (result.IsFailure)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Employee information not found."));
            }

            return Ok(ApiResponse<StaffProfileDto>.SuccessResponse(result.Value));
        }
    }
}
