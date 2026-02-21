using BookingService.Application.DTOs.RoomAssignment;
using BookingService.Application.Features.RoomAssignment.Commands.CheckInRoom;
using BookingService.Application.Features.RoomAssignment.Commands.CheckOutRoom;
using BookingService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace BookingService.API.Controllers
{
    [Route("api/room-assignments")]
    [ApiController]
    public class RoomAssignmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoomAssignmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Checkin cho khách hàng khi đến sảnh khách sạn 
        /// </summary>
        /// <remarks>
        /// Các role HotelOwner,Housekeeping được gọi
        /// </remarks>
        [Authorize(Roles = $"{AppRoles.HotelOwner},{AppRoles.Receptionist}")]
        [HttpPost("{bookingId}/check-in")]
        public async Task<IActionResult> CheckIn(Guid bookingId, [FromBody]CheckInRoomDto command)
        {
            var checkInCommand = new CheckInRoomCommand
            {
                BookingId = bookingId,
                HotelId = command.HotelId,
                RoomId = command.RoomId
            };

            var result = await _mediator.Send(checkInCommand);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "Booking.NotFound" => NotFound(ApiResponse<object>.ErrorResponse(result.Error.Message)),
                    "Room.NotAvailable" => BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message)),
                    "Mismatch" => BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message)),
                    "Overbooking" => BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message)),
                    "Assign.Failed" => BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message)),
                    _ => BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message))
                };
            }

            return Ok(ApiResponse<string>.SuccessResponse("Check-in completed successfully."));
        }

        /// <summary>
        /// Checkout cho khách hàng khi họ đã ở xong các ngày book
        /// </summary>
        /// <remarks>
        /// Các role HotelOwner,Housekeeping được gọi
        /// </remarks>
        [Authorize(Roles = $"{AppRoles.HotelOwner},{AppRoles.Receptionist}")]
        [HttpPost("{bookingId}/check-out")]
        public async Task<IActionResult> CheckOut(Guid bookingId, [FromBody]CheckOutRoomDto dto)
        {
            var command = new CheckOutRoomCommand
            {
                BookingId = bookingId,
                RoomId = dto.RoomId
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "Booking.NotFound" => NotFound(ApiResponse<object>.ErrorResponse(result.Error.Message)),
                    "Assignment.NotFound" => NotFound(ApiResponse<object>.ErrorResponse(result.Error.Message)),
                    "Domain.Error" => BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message)), 
                    _ => BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message))
                };
            }

            return Ok(ApiResponse<string>.SuccessResponse("Check-out successful. The room has been moved to waiting-for-cleaning status."));
        }

    }
}
