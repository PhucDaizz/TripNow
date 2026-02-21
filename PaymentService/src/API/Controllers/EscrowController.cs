using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.Features.EscrowAccount.Queries.GetEscrowByBookingId;
using PaymentService.Application.Features.EscrowAccount.Queries.GetEscrows;
using PaymentService.Domain.Common;

namespace PaymentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{AppRoles.SysAdmin}")]
    public class EscrowController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EscrowController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách thanh toán người dùng đã trả cho hệ thống
        /// </summary>
        /// <remarks>
        /// Chỉ admin được gọi 
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] GetEscrowsQuery query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse(result.Value)) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

        /// <summary>
        /// Xem chi tiết 1 thanh toán mà hệ thống đang giữ
        /// </summary>
        /// <remarks>
        /// Chỉ admin được gọi 
        /// </remarks>
        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetByBookingId(Guid bookingId)
        {
            var result = await _mediator.Send(new GetEscrowByBookingIdQuery(bookingId));
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse(result.Value)) : NotFound(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }



    }
}
