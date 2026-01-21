using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Booking;
using BookingService.Application.Features.Booking.Commands.CancelBooking;
using BookingService.Application.Features.Booking.Commands.CreateBooking;
using BookingService.Domain.Common;
using BookingService.Domain.Enum;
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
        [Authorize(Roles = $"{AppRoles.Customer}, {AppRoles.Receptionist}, {AppRoles.HotelOwner}, {AppRoles.SysAdmin}")]
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


        /// <summary>
        /// Huỷ đơn đặt phòng lúc chưa thanh toán (Admin được huỷ tất cả, chủ ks và lễ tân chỉ huỷ trong khách sạn 9 mình, user chỉ huỷ cúa mình)
        /// </summary>
        [HttpPost]
        [Route("cancel")]
        [Authorize(Roles = $"{AppRoles.Customer},{AppRoles.Receptionist},{AppRoles.HotelOwner},{AppRoles.SysAdmin}")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelUnpaidBookingDto command, CancellationToken cancellationToken)
        {
            var cancelledBy = ResolveCancelledBy(_currentUserService);

            var commandHotel = new CancelBookingCommand
            {
                BookingId = command.BookingId,
                CancelledBy = cancelledBy,
                RefundPolicy = RefundPolicy.NonRefundable,
                RefundAmount = 0,
                Reason = command.Reason
            };

            var result = await _mediator.Send(commandHotel, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
            }
            var apiResponse = ApiResponse<string>.SuccessResponse("Booking cancelled successfully.");
            return Ok(apiResponse);
        }

        /// <summary>
        /// Huỷ đơn đặt phòng lúc đã thanh toán nhưng chưa đến ngày ở(Admin được huỷ tất cả, chủ ks và lễ tân chỉ huỷ trong khách sạn 9 mình, user chỉ huỷ cúa mình)
        /// </summary>



        [HttpGet("{id}")]
        [ActionName(nameof(GetBookingById))]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            // Todo: Implement Get Query later
            return Ok();
        }


        private static CancelledBy ResolveCancelledBy(
            ICurrentUserService currentUser)
        {
            return currentUser.Role switch
            {
                AppRoles.Customer => CancelledBy.User,

                AppRoles.HotelOwner => CancelledBy.Hotel,
                AppRoles.Receptionist => CancelledBy.Hotel,

                AppRoles.SysAdmin => CancelledBy.System,

                _ => throw new InvalidOperationException(
                        $"Unsupported role: {currentUser.Role}")
            };
        }


    }
}
