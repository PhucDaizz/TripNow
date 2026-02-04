using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.DTOs.Payment;
using PaymentService.Application.Features.Payment.Commands.CreatePaymentLink;
using PaymentService.Application.Features.Payment.Commands.VnpayCallback;

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

        [HttpGet("proceed-after-payment")]
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

    }
}
