using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Floor;
using HotelCatalogService.Application.Features.Floor.Commands.CreateFloor;
using HotelCatalogService.Application.Features.Floor.Commands.DeleteFloor;
using HotelCatalogService.Application.Features.Floor.Commands.UpdateFloor;
using HotelCatalogService.Application.Features.Floor.Queries.GetFloorsByBlock;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.API.Controllers
{
    [Route("api/Hotel/{hotelId}/blocks/{blockId}/floors")]
    [ApiController]
    public class HotelFloorsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public HotelFloorsController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Xem dãy phòng của khách sạn có bao nhiêu tầng
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetFloors(Guid hotelId, Guid blockId)
        {
            var result = await _mediator.Send(new GetFloorsByBlockQuery { HotelId = hotelId, BlockId = blockId });
            return result.IsSuccess
                ? Ok(ApiResponse<List<FloorDto>>.SuccessResponse(result.Value))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Tạo tầng của khách sạn
        /// </summary>
        /// <remarks>
        /// Lưu ý:
        /// - Tạo dãy trước rồi mới đến tầng
        /// - Nên ưu tiên gọi api 2.1 ở lần đầu tiên
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "HotelOwner")]
        public async Task<IActionResult> Create(Guid hotelId, Guid blockId, [FromBody] CreateFloorRequest request)
        {
            var command = new CreateFloorCommand
            {
                HotelId = hotelId,
                BlockId = blockId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                FloorNumber = request.FloorNumber
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<Guid>.SuccessResponse(result.Value, "Thêm tầng thành công"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Cập nhật thông tin tầng
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPut("{floorId}")]
        [Authorize(Roles = "HotelOwner")]
        public async Task<IActionResult> Update(Guid hotelId, Guid blockId, Guid floorId, [FromBody] UpdateFloorRequest request)
        {
            var command = new UpdateFloorCommand
            {
                HotelId = hotelId,
                BlockId = blockId,
                FloorId = floorId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                FloorNumber = request.FloorNumber
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Cập nhật thành công"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Xoá tầng
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpDelete("{floorId}")]
        [Authorize(Roles = "HotelOwner")]
        public async Task<IActionResult> Delete(Guid hotelId, Guid blockId, Guid floorId)
        {
            var command = new DeleteFloorCommand
            {
                HotelId = hotelId,
                BlockId = blockId,
                FloorId = floorId,
                OwnerId = Guid.Parse(_currentUser.UserId)
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Xóa tầng thành công"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }
    }
}
