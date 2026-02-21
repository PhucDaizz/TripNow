using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.Features.RoomTypeImage.Commands.DeleteRoomTypeImage;
using HotelCatalogService.Application.Features.RoomTypeImage.Commands.SetMainRoomTypeImage;
using HotelCatalogService.Application.Features.RoomTypeImage.Commands.UploadRoomTypeImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.API.Controllers
{
    [Route("api/Hotel/{hotelId}/room-types/{roomTypeId}/images")]
    [ApiController]
    public class RoomTypeImagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public RoomTypeImagesController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Đăng tải nhiều ảnh về 1 loại phòng
        /// </summary>
        /// <remarks>
        /// Role HotelOwnerđược gọi
        /// </remarks>
        [HttpPost("bulk")]
        [Authorize(Roles = "HotelOwner")]
        public async Task<IActionResult> Upload(Guid hotelId, Guid roomTypeId, [FromForm] List<IFormFile> files)
        {
            var command = new UploadRoomTypeImagesCommand
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                ImageFiles = files
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<List<string>>.SuccessResponse(result.Value, "Upload successful"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Đặt ảnh thành ảnh chính của loại phòng
        /// </summary>
        /// <remarks>
        /// Role HotelOwner được gọi
        /// </remarks>
        [HttpPut("{imageId}/main")]
        [Authorize(Roles = "HotelOwner")]
        public async Task<IActionResult> SetMain(Guid hotelId, Guid roomTypeId, Guid imageId)
        {
            var command = new SetMainRoomTypeImageCommand
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                ImageId = imageId,
                OwnerId = Guid.Parse(_currentUser.UserId)
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "This image has been set as the primary image."))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Xoá 1 ảnh loại phòng
        /// </summary>
        /// <remarks>
        /// Role HotelOwner được gọi
        /// </remarks>
        [HttpDelete("{imageId}")]
        [Authorize(Roles = "HotelOwner")]
        public async Task<IActionResult> Delete(Guid hotelId, Guid roomTypeId, Guid imageId)
        {
            var command = new DeleteRoomTypeImageCommand
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                ImageId = imageId,
                OwnerId = Guid.Parse(_currentUser.UserId)
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Deleted photos successfully"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }
    }
}
