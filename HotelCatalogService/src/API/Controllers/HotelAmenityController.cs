using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.HotelAmenity;
using HotelCatalogService.Application.Features.HotelAmenity.Commands.AddHotelAmenity;
using HotelCatalogService.Application.Features.HotelAmenity.Commands.RemoveHotelAmenity;
using HotelCatalogService.Application.Features.HotelAmenity.Commands.UpdateHotelAmenity;
using HotelCatalogService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.API.Controllers
{
    [Route("api/Hotel/{hotelId}/amenities")]
    [ApiController]
    public class HotelAmenityController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public HotelAmenityController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Chủ khách sạn gắn tiện ích vào khách sạn của họ
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> AddAmenityToHotel(Guid hotelId, [FromBody]AddHotelAmenityRequest request)
        {
            var command = new AddHotelAmenityCommand
            {
                HotelId = hotelId,
                OwnerId = Guid.Parse(_currentUser.UserId),

                AmenityId = request.AmenityId,
                Description = request.Description,
                IsFree = request.IsFree 
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<object>.SuccessResponse(null, "Amenity added successfully."));
            }

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Chủ khách sạn cập nhật tiện ích khách sạn của họ
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPut("{amenityId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> UpdateAmenity(Guid hotelId, Guid amenityId, [FromBody] UpdateHotelAmenityRequest request)
        {
            var command = new UpdateHotelAmenityCommand
            {
                HotelId = hotelId,
                AmenityId = amenityId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                Description = request.Description,
                IsFree = request.IsFree
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(ApiResponse<object>.SuccessResponse(null, "Amenity has been successfully updated."));

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Chủ khách sạn xoá tiện ích khách sạn của họ
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpDelete("{amenityId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> RemoveAmenity(Guid hotelId, Guid amenityId)
        {
            var command = new RemoveHotelAmenityCommand
            {
                HotelId = hotelId,
                AmenityId = amenityId,
                OwnerId = Guid.Parse(_currentUser.UserId)
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(ApiResponse<object>.SuccessResponse(null, "Amenity removed successfully"));

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }
    }
}
