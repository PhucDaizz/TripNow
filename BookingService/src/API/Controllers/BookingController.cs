using BookingService.Application.Common.Interfaces;
using BookingService.Application.Features.Booking.Commands.CreateBooking;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace BookingService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public BookingController(ICurrentUserService currentUserService, IMediator mediator)
        {
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        /// <summary>
        /// Tạo mới đơn đặt phòng
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command, CancellationToken cancellationToken)
        {
            var bookingId = await _mediator.Send(command, cancellationToken);

            // 3. Chuẩn hóa Response trả về
            var response = ApiResponse<Guid>.SuccessResponse(bookingId, "Booking created successfully.");

            // 4. Trả về 201 Created + Header Location + Body
            // Header Location: api/bookings/{id}
            return CreatedAtAction(nameof(GetBookingById), new { id = bookingId }, response);
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetBookingById))]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            // Todo: Implement Get Query later
            return Ok();
        }


    }
}
