using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.HotelImage;
using HotelCatalogService.Application.Features.HotelImage.Commands.DeleteHotelImage;
using HotelCatalogService.Application.Features.HotelImage.Commands.UpdateHotelImage;
using HotelCatalogService.Application.Features.HotelImage.Commands.UploadImages;
using HotelCatalogService.Application.Features.HotelImage.Queries;
using HotelCatalogService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.API.Controllers
{
    [Route("api/Hotel/{hotelId}/images")]
    [ApiController]
    public class HotelImagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public HotelImagesController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách hình ảnh của một khách sạn (không cần quyền, ai cũng có thể xem được)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelImages(Guid hotelId, CancellationToken cancellationToken)
        {
            var query = new GetHotelImagesQuery { HotelId = hotelId };

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
            }

            return Ok(ApiResponse<List<HotelImageDto>>.SuccessResponse(result.Value));
        }


        /// <summary>
        /// Đăng tải nhiều ảnh về khách sạn (chú ý loại FromForm)
        /// </summary>
        /// <remarks>
        /// Cần quyền chủ khách sạn
        /// </remarks>
        [HttpPost("bulk")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> UploadMultipleImages(Guid hotelId, [FromForm] List<IFormFile> files)
        {
            var command = new UploadHotelImagesCommand
            {
                HotelId = hotelId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                ImageFiles = files
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<List<string>>.SuccessResponse(result.Value, $"Upload successful {result.Value.Count} images"));
            }

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Cập nhật thông tin, vị trí ảnh khách sạn
        /// </summary>
        /// <remarks>
        /// Cần quyền chủ khách sạn
        /// </remarks>
        [HttpPut("{imageId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> UpdateImageDetails(Guid hotelId, Guid imageId, [FromBody] UpdateImageRequest request)
        {
            var command = new UpdateHotelImageCommand
            {
                HotelId = hotelId,
                ImageId = imageId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                IsThumbnail = request.IsThumbnail,
                Caption = request.Caption
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<object>.SuccessResponse(null, "Update successfuly"));
            }

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Xoá ảnh khách sạn
        /// </summary>
        /// <remarks>
        /// Cần quyền chủ khách sạn
        /// </remarks>
        [HttpDelete("{imageId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> DeleteImage(Guid hotelId, Guid imageId)
        {
            var command = new DeleteHotelImageCommand
            {
                HotelId = hotelId,
                ImageId = imageId,
                OwnerId = Guid.Parse(_currentUser.UserId)
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<object>.SuccessResponse(null, "Xóa ảnh thành công"));
            }

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

    }
}
