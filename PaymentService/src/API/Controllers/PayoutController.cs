using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.Features.Payout.Commands.AcceptPayout;
using PaymentService.Application.Features.Payout.Commands.RejectPayout;
using PaymentService.Application.Features.Payout.Queries.GetOwnerPayouts;
using PaymentService.Application.Features.Payout.Queries.GetPayouts;
using PaymentService.Domain.Common;

namespace PaymentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayoutController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PayoutController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Xem dánh sách tiền hệ thống đã chi trả cho khách sạn
        /// </summary>
        /// <remarks>
        /// Chỉ admin dùng
        /// </remarks>
        [HttpGet("admin")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> GetPayouts([FromQuery] GetPayoutsQuery query)
        {
            if (!query.Status.HasValue) query.Status = Domain.Enum.PayoutStatus.Pending; 

            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        /// <summary>
        /// Admin từ chối thanh toán cho khách sạn
        /// </summary>
        /// <remarks>
        /// Chỉ admin dùng
        /// - Lý do VD: chủ khásh sạn chưa cậ nhật tài khoản thụ hưởng
        /// </remarks>
        [HttpPost("admin/reject")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> RejectPayout([FromBody]RejectPayoutCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<object>.SuccessResponse("Complete refund rejection"));
        }

        /// <summary>
        /// Admin chấp nhân thanh toán cho khách sạn 
        /// </summary>
        /// <remarks>
        /// Chỉ admin dùng
        /// - Lưu ý: Yêu cầu thanh toán thủ công cho chủ khách sạn bằng tay tước sau đó mới chấp nhận và điền thông tin cần thiết
        /// </remarks>
        [HttpPost("admin/accept")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> AcceptPayout([FromBody]AcceptPayoutCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<string>.SuccessResponse("Payment completed"));
        }

        /// <summary>
        /// Chủ khách sạn xem danh sách rút tiền và trạng thái
        /// </summary>
        /// <remarks>
        /// Chỉ chủ khách sạn dùng
        /// </remarks>
        [HttpGet("my-payouts")] 
        [Authorize(Roles = $"{AppRoles.HotelOwner}")] 
        public async Task<IActionResult> GetMyPayouts([FromQuery] GetOwnerPayoutsQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

    }
}
