using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Block;
using HotelCatalogService.Application.Features.Block.Commands.CreateBlock;
using HotelCatalogService.Application.Features.Block.Commands.DeleteBlock;
using HotelCatalogService.Application.Features.Block.Commands.UpdateBlock;
using HotelCatalogService.Application.Features.Block.Queries.GetBlocksByHotel;
using HotelCatalogService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.API.Controllers
{
    [Route("api/Hotel/{hotelId}/blocks")]
    [ApiController]
    public class HotelBlocksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public HotelBlocksController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlocks(Guid hotelId)
        {
            var result = await _mediator.Send(new GetBlocksByHotelQuery { HotelId = hotelId });
            return result.IsSuccess
                ? Ok(ApiResponse<List<BlockDto>>.SuccessResponse(result.Value))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        [HttpPost]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Create(Guid hotelId, [FromBody] CreateBlockRequest request)
        {
            var command = new CreateBlockCommand
            {
                HotelId = hotelId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                Name = request.Name
            };

            var result = await _mediator.Send(command);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetBlocks), new { hotelId }, ApiResponse<Guid>.SuccessResponse(result.Value))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        [HttpPut("{blockId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Update(Guid hotelId, Guid blockId, [FromBody] UpdateBlockRequest request)
        {
            var command = new UpdateBlockCommand
            {
                HotelId = hotelId,
                BlockId = blockId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                Name = request.Name
            };

            var result = await _mediator.Send(command);

            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Cập nhật thành công"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        [HttpDelete("{blockId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Delete(Guid hotelId, Guid blockId)
        {
            var command = new DeleteBlockCommand
            {
                HotelId = hotelId,
                BlockId = blockId,
                OwnerId = Guid.Parse(_currentUser.UserId)
            };

            var result = await _mediator.Send(command);

            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Xóa thành công"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }



    }
}
