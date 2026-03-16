using BookingService.Application.Features.Statistics.Queries.GetHotelDashboardSummary;
using BookingService.Application.Features.Statistics.Queries.GetOccupancyStatistics;
using BookingService.Application.Features.Statistics.Queries.GetRevenueStatistics;
using BookingService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace BookingService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{AppRoles.HotelOwner},{AppRoles.Receptionist},{AppRoles.SysAdmin}")]
    public class StatisticsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatisticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy tổng quan Dashboard của khách sạn theo ngày
        /// </summary>
        /// <remarks>
        /// Trả về các chỉ số quan trọng trong ngày: tổng booking đang hoạt động, lượt check-in/check-out hôm nay,
        /// số đơn chờ xác nhận, tổng doanh thu tích luỹ và doanh thu dự kiến từ đơn Pending.
        /// Ngoài ra còn trả về 5 đơn đặt phòng gần nhất (không bao gồm đơn đã huỷ).
        /// - HotelOwner: HotelId phải truyền vào, và khách sạn đó phải đúng là của mình sở hữu.
        /// - Receptionist: HotelId tự động lấy từ token, không cần truyền vào.
        /// - SysAdmin: bắt buộc truyền hotelId để xem dashboard của khách sạn cụ thể.
        /// - Mặc định date là ngày hôm nay nếu không truyền.
        /// </remarks>
        [HttpGet("hotel-summary")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetHotelSummary([FromQuery] Guid? hotelId, [FromQuery] DateOnly? date)
        {
            var query = new GetHotelDashboardSummaryQuery 
            { 
                HotelId = hotelId,
                Date = date ?? DateOnly.FromDateTime(DateTime.Today)
            };
            var result = await _mediator.Send(query);
            
            if (result.IsFailure)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
                
            return Ok(ApiResponse<object>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo khoảng thời gian
        /// </summary>
        /// <remarks>
        /// Trả về danh sách các điểm dữ liệu doanh thu (Label, Revenue, BookingCount) trong khoảng FromDate – ToDate.
        /// Hỗ trợ 2 kiểu nhóm qua tham số groupBy:
        /// - "Day" (mặc định): nhóm theo từng ngày, label định dạng "yyyy-MM-dd".
        /// - "Month": nhóm theo tháng, label định dạng "MM/yyyy".
        /// Chỉ tính các đơn có trạng thái Confirmed hoặc Completed.
        /// - HotelOwner: HotelId phải truyền vào, và khách sạn đó phải đúng là của mình sở hữu.
        /// - Receptionist: HotelId tự động lấy từ token, không cần truyền vào.
        /// - SysAdmin: có thể truyền hotelId để lọc hoặc bỏ trống để xem toàn hệ thống.
        /// - Mặc định FromDate = hôm nay - 30 ngày, ToDate = thời điểm hiện tại.
        /// </remarks>
        [HttpGet("revenue")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetRevenueStats(
            [FromQuery] Guid? hotelId, 
            [FromQuery] DateTime? fromDate, 
            [FromQuery] DateTime? toDate, 
            [FromQuery] string groupBy = "Day")
        {
            var query = new GetRevenueStatisticsQuery
            {
                HotelId = hotelId,
                FromDate = fromDate ?? DateTime.Today.AddDays(-30),
                ToDate = toDate ?? DateTime.Now,
                GroupBy = groupBy
            };
            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));

            return Ok(ApiResponse<object>.SuccessResponse(result.Value));
        }

        /// <summary>
        /// Lấy thống kê công suất phòng (Occupancy) theo khoảng ngày
        /// </summary>
        /// <remarks>
        /// Trả về danh sách các điểm dữ liệu công suất (Label, RoomsBooked, OccupancyRate) từ bảng Inventory.
        /// OccupancyRate được tính theo công thức: SoldStock / TotalStock * 100 (%).
        /// Dữ liệu được nhóm theo từng ngày, label định dạng "yyyy-MM-dd", sắp xếp tăng dần theo ngày.
        /// - HotelOwner: HotelId phải truyền vào, và khách sạn đó phải đúng là của mình sở hữu.
        /// - Receptionist: HotelId tự động lấy từ token.
        /// - SysAdmin: bắt buộc truyền hotelId để xem công suất của khách sạn cụ thể.
        /// - Mặc định FromDate = hôm nay, ToDate = hôm nay + 14 ngày.
        /// - Trả về danh sách rỗng nếu khách sạn chưa có booking nào.
        /// </remarks>
        [HttpGet("occupancy")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetOccupancyStats(
            [FromQuery] Guid? hotelId, 
            [FromQuery] DateOnly? fromDate, 
            [FromQuery] DateOnly? toDate)
        {
            var query = new GetOccupancyStatisticsQuery
            {
                HotelId = hotelId,
                FromDate = fromDate ?? DateOnly.FromDateTime(DateTime.Today),
                ToDate = toDate ?? DateOnly.FromDateTime(DateTime.Today.AddDays(14))
            };
            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));

            return Ok(ApiResponse<object>.SuccessResponse(result.Value));
        }
    }
}
