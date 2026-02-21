using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.RefundRequest;
using PaymentService.Application.Features.RefundRequest.Commands.CompleteRefund;
using PaymentService.Application.Features.RefundRequest.Queries.GetRefundRequest;
using PaymentService.Domain.Common;

namespace PaymentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public RefundController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Admin xem danh sách hoàn tiền 
        /// </summary>
        /// <remarks>
        /// - API này cung cấp danh sách số tiền sẽ hoàn lại cho khách hàng nếu họ đã thanh toán và đặt nhầm hoặc danh sách hoàn 1 phần do huỷ gần sát giờ checkin
        /// - Chỉ admin dùng
        /// </remarks>
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        [HttpGet]
        public async Task<IActionResult> GetRequests([FromQuery] GetRefundRequestsQuery query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse(result)) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

        /// <summary>
        /// Người dùng xem danh sách hoàn tiền của họ
        /// </summary>
        /// <remarks>
        /// Chỉ người dùng sử dụng
        /// </remarks>
        [Authorize] 
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests([FromQuery] GetRefundRequestsQuery query)
        {
            var userIdString = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            query.UserId = Guid.Parse(userIdString);

            var result = await _mediator.Send(query);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(result.Value))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

        /// <summary>
        /// Admin xác nhận đã hoàn trả tiền thành công
        /// </summary>
        /// <remarks>
        /// Sau khi Admin chuyển khoản hoặc hoàn tiền qua cổng thanh toán xong, gọi API này để cập nhật trạng thái yêu cầu hoàn tiền thành Completed.
        /// </remarks>
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteRefund(Guid id, [FromBody] CompleteRefundRequestDto request)
        {
            var command = new CompleteRefundCommand
            {
                RefundRequestId = id,
                RefundGatewayTransactionRef = request.RefundGatewayTransactionRef,
                Note = request.Note
            };

            var result = await _mediator.Send(command);

            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse("Refund successfully confirmed."))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

    }
}
