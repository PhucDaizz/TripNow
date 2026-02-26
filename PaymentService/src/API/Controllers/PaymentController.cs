using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.DTOs.Payment;
using PaymentService.Application.Features.Payment.Commands.CreatePaymentLink;
using PaymentService.Application.Features.Payment.Commands.VnpayCallback;
using PaymentService.Application.Features.Payment.Queries.GetTransactionById;
using PaymentService.Application.Features.Payment.Queries.GetTransactions;
using PaymentService.Domain.Common;

namespace PaymentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tạo link thanh toán VNPAY
        /// </summary>
        /// <remarks>
        /// Khách hàng gọi
        /// </remarks>
        [HttpPost("payment-link")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentLinkDTO createPaymentLinkDTO)
        {
            var command = new CreatePaymentLinkCommand
            {
                providerBank = createPaymentLinkDTO.providerBank,
                BookingId = createPaymentLinkDTO.BookingId,
                MoneyToPay = createPaymentLinkDTO.MoneyToPay
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(ApiResponse<string>.SuccessResponse(result.Value));
            }
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));

        }

        /// <summary>
        /// Hệ thống của VNPAY tự động gọi vào API này để xử lý thành toán
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("proceed-after-payment")]
        [AllowAnonymous]
        public async Task<IActionResult> ProceedAfterPayment()
        {
            var rawQuery = Request.Query; 

            var parameters = rawQuery.ToDictionary(
                x => x.Key,
                x => x.Value.ToString());

            var result = await _mediator.Send(
                new VnpayCallbackCommand(parameters));

            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        /// <summary>
        /// Xem lịch sử thanh toán - sổ cái
        /// </summary>
        /// <remarks>
        /// - Chỉ admin dùng
        /// </remarks>
        [HttpGet("history")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")] 
        public async Task<IActionResult> GetHistory([FromQuery] GetTransactionsQuery query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(ApiResponse<Domain.Common.Models.PagedResult<PaymentTransactionDto>>.SuccessResponse(result.Value)) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }
        /// <summary>
        /// Xem chi tiết thanh toán 
        /// </summary>
        /// <remarks>
        /// Chỉ Admin dùng
        /// </remarks>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _mediator.Send(new GetTransactionByIdQuery(id));
            return result.IsSuccess ? Ok(ApiResponse<PaymentTransactionDto>.SuccessResponse(result.Value)) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

    }
}
