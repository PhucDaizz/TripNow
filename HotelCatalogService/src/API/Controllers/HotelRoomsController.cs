using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Application.Features.Room.Commands.CreateRoom;
using HotelCatalogService.Application.Features.Room.Commands.DeleteRoom;
using HotelCatalogService.Application.Features.Room.Commands.FinishedMaintainRoom;
using HotelCatalogService.Application.Features.Room.Commands.MaintainRoom;
using HotelCatalogService.Application.Features.Room.Commands.UpdateRoom;
using HotelCatalogService.Application.Features.Room.Commands.UpdateRoomStatus;
using HotelCatalogService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.API.Controllers
{
    [Route("api/Hotel/{hotelId}/blocks/{blockId}/floors/{floorId}/rooms")]
    [ApiController]
    public class HotelRoomsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public HotelRoomsController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Tạo phòng
        /// </summary>
        /// <remarks>
        /// Cần quyền chủ khách sạn
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Create(Guid hotelId, Guid blockId, Guid floorId, [FromBody] CreateRoomRequest request)
        {
            var command = new CreateRoomCommand
            {
                HotelId = hotelId,
                BlockId = blockId,
                FloorId = floorId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                Name = request.Name,
                RoomTypeId = request.RoomTypeId
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<Guid>.SuccessResponse(result.Value, "Add room successfully"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Cập nhật phòng
        /// </summary>
        /// <remarks>
        /// Cần quyền chủ khách sạn
        /// </remarks>
        [HttpPut("{roomId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Update(Guid hotelId, Guid blockId, Guid floorId, Guid roomId, [FromBody] UpdateRoomRequest request)
        {
            var command = new UpdateRoomCommand
            {
                HotelId = hotelId,
                BlockId = blockId,
                FloorId = floorId,
                RoomId = roomId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                Name = request.Name,
                RoomTypeId = request.RoomTypeId
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Update successfully"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Thay đổi trạng thái phòng
        /// </summary>
        /// <remarks>
        /// Cần quyền chủ khách sạn
        /// </remarks>
        [HttpPatch("{roomId}/status")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> UpdateStatus(Guid hotelId, Guid blockId, Guid floorId, Guid roomId, [FromBody] UpdateRoomStatusRequest request)
        {
            var command = new UpdateRoomStatusCommand
            {
                HotelId = hotelId,
                BlockId = blockId,
                FloorId = floorId,
                RoomId = roomId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                NewStatus = request.Status
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Update status successfully"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Chuyển trạng thái sửa chửa phòng
        /// </summary>
        /// <remarks>
        /// Cần quyền chủ khách sạn
        /// </remarks>
        [HttpPatch("{roomId}/maintain")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Maintain(Guid hotelId, Guid blockId, Guid floorId, Guid roomId, [FromBody] MaintainRoomRequest request)
        {
            var command = new MaintainRoomCommand
            {
                HotelId = hotelId,
                BlockId = blockId,
                FloorId = floorId,
                RoomId = roomId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                FromDate = request.FromDate,
                ToDate = request.ToDate
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Update status successfully"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        ///  Chuyển trạng thái sửa chửa phòng hoàn tất
        /// </summary>
        /// <remarks>
        /// Cần quyền chủ khách sạn
        /// </remarks>
        [HttpPatch("{roomId}/finished-maintain")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> FinishedMaintain(Guid hotelId, Guid blockId, Guid floorId, Guid roomId)
        {
            var command = new FinishedMaintainRoomCommand
            {
                HotelId = hotelId,
                BlockId = blockId,
                FloorId = floorId,
                RoomId = roomId,
                OwnerId = Guid.Parse(_currentUser.UserId)
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Update status successfully"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Xoá phòng
        /// </summary>
        /// <remarks>
        /// Cần quyền chủ khách sạn
        /// </remarks>
        [HttpDelete("{roomId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Delete(Guid hotelId, Guid blockId, Guid floorId, Guid roomId)
        {
            var command = new DeleteRoomCommand
            {
                HotelId = hotelId,
                BlockId = blockId,
                FloorId = floorId,
                RoomId = roomId,
                OwnerId = Guid.Parse(_currentUser.UserId)
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Delete room successfully"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }
    }
}
