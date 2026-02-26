using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Block;
using HotelCatalogService.Application.DTOs.RoomPrice;
using HotelCatalogService.Application.DTOs.RoomType;
using HotelCatalogService.Application.Features.Room.Queries.GetAvailableRoomsTree;
using HotelCatalogService.Application.Features.RoomPrice.Queries.GetRoomTypeCalendar;
using HotelCatalogService.Application.Features.RoomType.Commands.CreateRoomType;
using HotelCatalogService.Application.Features.RoomType.Commands.DeleteRoomType;
using HotelCatalogService.Application.Features.RoomType.Commands.RemovePolicy;
using HotelCatalogService.Application.Features.RoomType.Commands.SetPolicy;
using HotelCatalogService.Application.Features.RoomType.Commands.UpdateRoomType;
using HotelCatalogService.Application.Features.RoomType.Queries.GetRoomTypesByHotel;
using HotelCatalogService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.API.Controllers
{
    [Route("api/Hotel/{hotelId}/room-types")]
    [ApiController]
    public class HotelRoomTypesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public HotelRoomTypesController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Lấy danh sách các loại phòng của khách sạn, có thể lọc theo ngày check-in.
        /// </summary>
        /// <param name="hotelId">ID của khách sạn.</param>
        /// <param name="checkInDate">Ngày check-in (tùy chọn).</param>
        /// <returns>Danh sách các loại phòng.</returns>
        [HttpGet]
        public async Task<IActionResult> GetRoomTypes(Guid hotelId, [FromQuery] DateTime? checkInDate)
        {
            var result = await _mediator.Send(new GetRoomTypesByHotelQuery
            {
                HotelId = hotelId,
                CheckInDate = checkInDate
            });
            return result.IsSuccess
                ? Ok(ApiResponse<List<RoomTypeDto>>.SuccessResponse(result.Value))
                : BadRequest(ApiResponse<object>.ErrorResponse("", new List<string> { result.Error.Message }));
        }


        /// <summary>
        /// Lấy danh sách phân cấp các phòng còn trống cho một khách sạn và loại phòng được chỉ định
        /// </summary>
        /// <remarks>
        /// Chỉ chủ khách sạn và lễ tân được gọi
        /// </remarks>
        [HttpGet("tree")]
        [Authorize(Roles = $"{AppRoles.HotelOwner},{AppRoles.Receptionist}")]
        public async Task<IActionResult> GetAvailableRoomsTree(Guid hotelId, Guid roomTypeId)
        {
            if (_currentUser.Role == AppRoles.Receptionist)
            {
                var tokenHotelId = _currentUser.HotelId;
                if (tokenHotelId != null && tokenHotelId != hotelId)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        ApiResponse<object>.ErrorResponse("Receptionist can only access their assigned hotel."));
                }
            }

            var query = new GetAvailableRoomsTreeQuery
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess
                ? Ok(ApiResponse<List<BlockResponse>>.SuccessResponse(result.Value))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Xem lịch giá của một loại phòng trong tháng cụ thể.
        /// </summary>
        [HttpGet("calendar")]
        public async Task<IActionResult> GetPriceCalendar(Guid hotelId, Guid roomTypeId, [FromQuery] int month, [FromQuery] int year)
        {
            if (month == 0 || year == 0)
            {
                month = DateTime.Today.Month;
                year = DateTime.Today.Year;
            }

            var query = new GetRoomTypeCalendarQuery
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                Month = month,
                Year = year
            };

            var result = await _mediator.Send(query);

            return result.IsSuccess
                ? Ok(ApiResponse<List<CalendarDayDto>>.SuccessResponse(result.Value))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }


        /// <summary>
        /// 2.3 Tạo mới một loại phòng cho khách sạn (Yêu cầu quyền chủ khách sạn).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Create(Guid hotelId, [FromBody] CreateRoomTypeRequest request)
        {
            var command = new CreateRoomTypeCommand
            {
                HotelId = hotelId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                Name = request.Name,
                BasePrice = request.BasePrice,
                Capacity = request.Capacity,
                SizeM2 = request.SizeM2
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetRoomTypes), new { hotelId }, ApiResponse<Guid>.SuccessResponse(result.Value));

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }


        /// <summary>
        /// Cập nhật thông tin của một loại phòng (Yêu cầu quyền chủ khách sạn).
        /// </summary>
        [HttpPut("{roomTypeId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Update(Guid hotelId, Guid roomTypeId, [FromBody] UpdateRoomTypeRequest request)
        {
            var command = new UpdateRoomTypeCommand
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                Name = request.Name,
                BasePrice = request.BasePrice,
                Capacity = request.Capacity,
                SizeM2 = request.SizeM2
            };

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse(null, "Update success")) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }


        /// <summary>
        /// Xóa một loại phòng khỏi hệ thống (Yêu cầu quyền chủ khách sạn).
        /// </summary>
        [HttpDelete("{roomTypeId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Delete(Guid hotelId, Guid roomTypeId)
        {
            var command = new DeleteRoomTypeCommand
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                OwnerId = Guid.Parse(_currentUser.UserId)
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<object>.SuccessResponse(null, "Room type deleted successfully."));
            }

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Thêm hoặc Cập nhật chính sách hủy cho loại phòng (Yêu cầu quyền chủ khách sạn).
        /// </summary>
        [HttpPut("{roomTypeId}/policy")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> SetPolicy(Guid hotelId, Guid roomTypeId, [FromBody] SetPolicyRequest request)
        {
            var command = new SetRoomTypePolicyCommand
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                PolicyId = request.PolicyId
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess 
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Policy updated successfully.")) 
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Xóa chính sách hủy khỏi loại phòng (Yêu cầu quyền chủ khách sạn).
        /// </summary>
        [HttpDelete("{roomTypeId}/policy")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> RemovePolicy(Guid hotelId, Guid roomTypeId)
        {
            var command = new RemoveRoomTypePolicyCommand
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                OwnerId = Guid.Parse(_currentUser.UserId)
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess 
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Policy removed successfully.")) 
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }
    }
}
