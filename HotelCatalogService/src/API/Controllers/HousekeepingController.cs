using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Application.Features.Room.Commands.UpdateRoomStatus;
using HotelCatalogService.Application.Features.Room.Queries.GetDirtyRooms;
using HotelCatalogService.Domain.Common;
using HotelCatalogService.Domain.Dto.Room;
using HotelCatalogService.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.API.Controllers
{
    [Route("api/Hotel/{hotelId}/housekeeping")]
    [ApiController]
    public class HousekeepingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public HousekeepingController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Lấy danh sách phòng dơ
        /// </summary>
        /// <remarks>
        /// Các role HotelOwner,Housekeeping,SysAdmin được gọi
        /// </remarks>
        [HttpGet("dirty-rooms")]
        [Authorize(Roles = $"{AppRoles.HotelOwner},{AppRoles.Housekeeping},{AppRoles.SysAdmin}")] 
        public async Task<IActionResult> GetDirtyRooms(Guid hotelId, [FromQuery] Guid? blockId, [FromQuery] Guid? floorId)
        {
            var query = new GetDirtyRoomsQuery
            {
                HotelId = hotelId,
                BlockId = blockId,
                FloorId = floorId
            };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<DirtyRoomDto>>.SuccessResponse(result.Value));
        }


        /// <summary>
        /// Chuyển phòng sang trạng thái đang dọn
        /// </summary>
        /// <remarks>
        /// Các role HotelOwner,Housekeeping được gọi
        /// </remarks>
        [HttpPost("rooms/{roomId}/start")]
        [Authorize(Roles = $"{AppRoles.HotelOwner},{AppRoles.Housekeeping}")]
        public async Task<IActionResult> StartCleaning(Guid hotelId, Guid roomId, [FromBody] RoomLocationRequest request)
        {
            var command = new UpdateRoomStatusCommand
            {
                HotelId = hotelId,
                BlockId = request.BlockId,
                FloorId = request.FloorId,
                RoomId = roomId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                NewStatus = RoomStatus.Cleaning 
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(ApiResponse<object>.SuccessResponse(null, "Đã bắt đầu dọn phòng (Status: Cleaning)"));

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Chuyển phòng sang trang thái đã dọn có thể tiếp nhận khách mới
        /// </summary>
        /// <remarks>
        /// Các role HotelOwner,Housekeeping,SysAdmin được gọi
        /// </remarks>
        [HttpPost("rooms/{roomId}/finish")]
        [Authorize(Roles = $"{AppRoles.HotelOwner},{AppRoles.Housekeeping}")]
        public async Task<IActionResult> FinishCleaning(Guid hotelId, Guid roomId, [FromBody] RoomLocationRequest request)
        {
            var command = new UpdateRoomStatusCommand
            {
                HotelId = hotelId,
                BlockId = request.BlockId,
                FloorId = request.FloorId,
                RoomId = roomId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                NewStatus = RoomStatus.Available 
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(ApiResponse<object>.SuccessResponse(null, "Đã dọn xong (Status: Available)"));

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }
    }
}

