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

        [HttpGet("hotel-summary")]
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

        [HttpGet("revenue")]
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

        [HttpGet("occupancy")]
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
