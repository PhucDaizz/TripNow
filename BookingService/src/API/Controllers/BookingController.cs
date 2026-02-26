using BookingService.Application.Common.Interfaces;
using BookingService.Application.DTOs.Booking;
using BookingService.Application.Features.Booking.Commands.CancelBooking;
using BookingService.Application.Features.Booking.Commands.CreateBooking;
using BookingService.Application.Features.Booking.Queries.GetBookings;
using BookingService.Application.Features.Booking.Queries.GetDetailBooking;
using BookingService.Application.Features.Booking.Queries.IsBookingExisting;
using BookingService.Application.Features.Inventory.Queries.CheckRoomUsage;
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
        /// <remarks>
        /// Tất cả role được gọi - tốt nhất nên dùng role khách hàng 
        /// </remarks>
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
        /// Huỷ đơn đặt phòng (Admin được huỷ tất cả, chủ ks và lễ tân chỉ huỷ trong khách sạn 9 mình, user chỉ huỷ cúa mình)
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
        /// Xem chi tiết đơn đặt phòng
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("{id}")]
        [ActionName(nameof(GetBookingById))]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            var query = new GetDetailBookingQuery
            {
                BookingId = id,
                UserId = Guid.Parse(_currentUserService.UserId)
            };
            var result =  await _mediator.Send(query);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
            }

            return Ok(ApiResponse<BookingDetailResponse>.SuccessResponse(result.Value, "Booking retrieved successfully."));
        }

        /// <summary>
        /// Lấy danh sách đơn đặt phòng (Filter, Paging, Sort)
        /// </summary>
        /// <remarks>
        /// API này tự động nhận diện Role của user (Admin, Owner, Customer) để trả về dữ liệu tương ứng.
        /// </remarks>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<BookingSummaryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBookings([FromQuery] GetBookingsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    message: result.Error.Message,
                    errors: new List<string> { result.Error.Code }
                ));
            }

            var response = ApiResponse<Domain.Common.Models.PagedResult<BookingSummaryDto>>.SuccessResponse(
                data: result.Value,
                message: "Booking list retrieved successfully."
            );

            return Ok(response);
        }

        /// <summary>
        /// Kiểm tra loại phòng có được book trong tương lại hay không (không map ra gateway)
        /// </summary>
        /// <remarks>
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("check-room-usage/{roomTypeId}")] 
        public async Task<IActionResult> CheckRoomUsage(Guid roomTypeId)
        {
            var result = await _mediator.Send(new CheckRoomUsageQuery { RoomTypeId = roomTypeId });
            if (result)
            {
                return Ok(ApiResponse<bool>.SuccessResponse(result)); 
            }
            return BadRequest(ApiResponse<bool>.SuccessResponse(result));
        }
        /// <summary>
        /// Kiểm ta người dùng đã từng ở đây chưa ( không map ra gateway )
        /// </summary>
        /// <remarks>
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("is-existing")] 
        public async Task<IActionResult> IsBookingExisting([FromQuery]Guid bookingId, [FromQuery]Guid userId)
        {
            var result = await _mediator.Send(new IsBookingExistingQuery { BookingId = bookingId });
            return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
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
