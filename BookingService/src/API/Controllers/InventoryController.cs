using BookingService.Application.DTOs.Inventory;
using BookingService.Application.Features.Booking.Queries.GetAvailability;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace BookingService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Truy vấn danh sách số lượng phòng còn trống của từng loại phòng
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("availability")]
        public async Task<IActionResult> GetAvailability([FromQuery] GetAvailabilityQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<List<DailyAvailabilityDto>>.SuccessResponse(
                    result.Value,
                    "Lấy dữ liệu lịch thành công"
                ));
            }

            return BadRequest(ApiResponse<List<DailyAvailabilityDto>>.ErrorResponse(
                result.Error.Message,
                new List<string> { result.Error.Code }
            ));
        }
    }
}
