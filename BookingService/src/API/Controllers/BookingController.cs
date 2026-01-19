using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Booking;
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
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
            }
            var bookingResponse = result.Value;

            var apiResponse = ApiResponse<CreateBookingResponse>.SuccessResponse(bookingResponse, "Booking created successfully.");

            return CreatedAtAction(
                nameof(GetBookingById),
                new { id = bookingResponse.BookingId },
                apiResponse
            );

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
