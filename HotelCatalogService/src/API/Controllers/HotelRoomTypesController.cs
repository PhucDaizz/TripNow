using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.RoomType;
using HotelCatalogService.Application.Features.Hotel.Commands.CreateRoomType;
using HotelCatalogService.Application.Features.Hotel.Commands.DeleteRoomType;
using HotelCatalogService.Application.Features.Hotel.Commands.UpdateRoomType;
using HotelCatalogService.Application.Features.Hotel.Queries.GetRoomTypesByHotel;
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

        [HttpGet]
        public async Task<IActionResult> GetRoomTypes(Guid hotelId)
        {
            var result = await _mediator.Send(new GetRoomTypesByHotelQuery { HotelId = hotelId });
            return result.IsSuccess ? Ok(ApiResponse<List<RoomTypeDto>>.SuccessResponse(result.Value)) : BadRequest(ApiResponse<object>.ErrorResponse("", new List<string> { result.Error.Message }));
        }


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
    }
}
