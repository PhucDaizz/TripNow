using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.RoomPrice;
using HotelCatalogService.Application.Features.RoomPrice.Commands.BulkSetRoomPrice;
using HotelCatalogService.Application.Features.RoomPrice.Commands.DeleteRoomPrice;
using HotelCatalogService.Application.Features.RoomPrice.Commands.SetRoomPrice;
using HotelCatalogService.Application.Features.RoomPrice.Queries.GetHotelBatchRoomPrices;
using HotelCatalogService.Application.Features.RoomPrice.Queries.GetRoomPrices;
using HotelCatalogService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.API.Controllers
{
    [Route("api/Hotel/{hotelId}/room-types/{roomTypeId}/prices")]
    [ApiController]
    public class RoomPricesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public RoomPricesController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Lấy danh sách giá phòng của tất cả loại phòng trong một khoảng thời gian.
        /// </summary>
        [HttpGet("/api/Hotel/{hotelId}/room-types/prices")]
        public async Task<IActionResult> GetBatchRoomPrices(Guid hotelId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var query = new GetHotelBatchRoomPricesQuery
            {
                HotelId = hotelId,
                FromDate = from,
                ToDate = to
            };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<RoomTypeCalendarDto>>.SuccessResponse(result.Value));
        }


        /// <summary>
        /// Lấy danh sách giá phòng ngày đặt biệt kể thường trong một khoảng thời gian. (ưu tiên đặc biệt)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPrices(Guid hotelId, Guid roomTypeId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var query = new GetRoomPricesQuery
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                FromDate = from,
                ToDate = to
            };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<RoomPriceDto>>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Thiết lập giá phòng cho một ngày cụ thể (Yêu cầu quyền chủ khách sạn).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> SetPrice(Guid hotelId, Guid roomTypeId, [FromBody] SetPriceRequest request)
        {
            var command = new SetRoomPriceCommand
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                Date = request.Date,
                Price = request.Price
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Prices have been updated."))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Xóa giá đặc biệt của một ngày, quay về giá gốc (Yêu cầu quyền chủ khách sạn).
        /// </summary>
        [HttpDelete("{date}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> DeletePrice(Guid hotelId, Guid roomTypeId, DateTime date)
        {
            var command = new DeleteRoomPriceCommand
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                Date = date
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "The special price has been removed and the price has reverted to the original price."))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Thiết lập giá phòng hàng loạt cho một khoảng thời gian (Yêu cầu quyền chủ khách sạn).
        /// </summary>
        [HttpPost("bulk")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> BulkSetPrice(Guid hotelId, Guid roomTypeId, [FromBody] BulkSetPriceRequest request)
        {
            var command = new BulkSetRoomPriceCommand
            {
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                OwnerId = Guid.Parse(_currentUser.UserId),
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Price = request.Price,
                SpecificDays = request.SpecificDays 
            };

            var result = await _mediator.Send(command);

            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Bulk price update successful"))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }
    }
}
