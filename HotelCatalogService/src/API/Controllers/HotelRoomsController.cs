using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Application.Features.Room.Commands.CreateRoom;
using HotelCatalogService.Application.Features.Room.Commands.DeleteRoom;
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
